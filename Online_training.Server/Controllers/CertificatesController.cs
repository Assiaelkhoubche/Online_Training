using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_training.Server.Models;
using Online_training.Server.Models.DTOs;
using System.Security.Claims;
using Online_training.Server.Certificate_Generation;

namespace E_Learning.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly CertificateGenerator _certificateGenerator;
        private readonly IWebHostEnvironment _environment;

        // Updated constructor to include necessary dependencies
        public CertificatesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
            // Initialize the certificate generator with the template path
            _certificateGenerator = new CertificateGenerator(
                Path.Combine(environment.WebRootPath, "templates", "certificate-template.pdf"));
        }

        // Existing GET endpoint remains unchanged
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Certificate>>> GetCertificates()
        {
            var participantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _context.Certificates
                .Where(c => c.ParticipantId == participantId)
                .Include(c => c.Formation)
                .ToListAsync();
        }

        // Updated GET by ID endpoint to include file download capability
        [HttpGet("{id}")]
        public async Task<ActionResult<Certificate>> GetCertificate(int id)
        {
            var participantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var certificate = await _context.Certificates
                .Include(c => c.Formation)
                .FirstOrDefaultAsync(c => c.Id == id && c.ParticipantId == participantId);

            if (certificate == null)
            {
                return NotFound(new { Message = "Certificate not found or access denied." });
            }

            // If the certificate file exists, include the download URL
            if (!string.IsNullOrEmpty(certificate.FilePath))
            {
                var fileUrl = $"/certificates/{Path.GetFileName(certificate.FilePath)}";
                certificate.DownloadUrl = fileUrl;
            }

            return certificate;
        }

        // Updated Generate endpoint to create PDF certificate
        [HttpPost("Generate/{formationId}")]
        public async Task<IActionResult> GenerateCertificate(int formationId)
        {
            var participantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Get formation with trainer information
            var formation = await _context.Formations
                .Include(f => f.Trainer)
                .FirstOrDefaultAsync(f => f.Id == formationId);

            if (formation == null)
            {
                return NotFound(new { Message = "Formation not found" });
            }

            // Get participant information
            var participant = await _context.Users.FindAsync(participantId);
            if (participant == null)
            {
                return NotFound(new { Message = "Participant not found" });
            }

            // Check progress
            var participantFormation = await _context.ParticipantFormations
                .FirstOrDefaultAsync(pf => pf.ParticipantId == participantId && pf.FormationId == formationId);
            if (participantFormation == null || participantFormation.Progress < 100)
            {
                return BadRequest(new { Message = "Certificate cannot be issued. Progress is incomplete." });
            }

            // Check if the certificate already exists
            var existingCertificate = await _context.Certificates
                .FirstOrDefaultAsync(c => c.ParticipantId == participantId && c.FormationId == formationId);
            if (existingCertificate != null)
            {
                // Send the existing certificate as the response
                var existingCertification = new Certification
                {
                    CertificateId = existingCertificate.Id,
                    ParticipantId = participantId,
                    FormationId = formationId,
                    DateIssued = existingCertificate.DateIssued,
                    FormationTitle = formation.Title,
                    InstructorName = formation.Trainer.UserName,
                    DownloadUrl = $"/certificates/{existingCertificate.FilePath}"
                };

                return Ok(existingCertification);
            }
            else
            {
                try
                {
                    // Create certificate record
                    var certificate = new Certificate
                    {
                        ParticipantId = participantId,
                        FormationId = formationId,
                        DateIssued = DateTime.UtcNow
                    };

                    _context.Certificates.Add(certificate);
                    await _context.SaveChangesAsync();

                    // Prepare certificate data
                    var certificateData = new CertificateData
                    {
                        ParticipantName = participant.UserName,
                        CourseTitle = formation.Title,
                        InstructorName = formation.Trainer.UserName,
                        DateIssued = certificate.DateIssued
                    };

                    // Generate PDF certificate
                    byte[] pdfBytes = _certificateGenerator.GenerateCertificate(certificateData);

                    // Create certificates directory if it doesn't exist
                    var certificatesPath = Path.Combine(_environment.WebRootPath, "certificates");
                    Directory.CreateDirectory(certificatesPath);

                    // Save the PDF file
                    var fileName = $"certificate_{certificate.Id}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                    var filePath = Path.Combine(certificatesPath, fileName);
                    await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);

                    // Update certificate record with file path
                    certificate.FilePath = fileName;
                    await _context.SaveChangesAsync();

                    // Create certification object
                    var certification = new Certification
                    {
                        CertificateId = certificate.Id,
                        ParticipantId = participantId,
                        FormationId = formationId,
                        DateIssued = certificate.DateIssued,
                        FormationTitle = formation.Title,
                        InstructorName = formation.Trainer.UserName,
                        DownloadUrl = $"/certificates/{fileName}"
                    };

                    return Ok(certification);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Message = "Failed to generate certificate", Error = ex.Message });
                }
            }
        }

        // New endpoint to download certificate
        [HttpGet("Download/{id}")]
        public async Task<IActionResult> DownloadCertificate(int id)
        {
            var participantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var certificate = await _context.Certificates
                .FirstOrDefaultAsync(c => c.Id == id && c.ParticipantId == participantId);

            if (certificate == null)
            {
                return NotFound(new { Message = "Certificate not found or access denied." });
            }

            if (string.IsNullOrEmpty(certificate.FilePath))
            {
                return NotFound(new { Message = "Certificate file not found." });
            }

            var filePath = Path.Combine(_environment.WebRootPath, "certificates", certificate.FilePath);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { Message = "Certificate file not found." });
            }

            var memory = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(memory, "application/pdf", certificate.FilePath);
        }
    }
}