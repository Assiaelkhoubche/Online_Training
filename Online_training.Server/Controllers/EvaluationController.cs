using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;
using Online_training.Server.Models;
using System.Security.Claims;
using Online_training.Server.Models.DTOs;
using iText.StyledXmlParser.Jsoup.Select;
using Microsoft.VisualBasic;

namespace E_Learning.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EvaluationController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost("add-review")]
        public async Task<IActionResult> AddEvaluation([FromBody] EvaluationModel model)
        {
            var participantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (participantId == null)
            {
                return BadRequest("Participant ID not found.");
            }
            
            var formation = await _context.Formations.FindAsync(model.FormationId);
            if (formation == null)
            {
                return NotFound("Formation not found.");
            }
            // Check if the participant has already evaluated the Formation
            var existingEvaluation = await _context.Evaluations
                .Where(e => e.ParticipantId == participantId && e.FormationId == model.FormationId)
                .FirstOrDefaultAsync();

            if (existingEvaluation != null)
            {
                // Update existing evaluation
                existingEvaluation.Rating = model.Rating;
                existingEvaluation.Review = model.Review;
                await _context.SaveChangesAsync();
                return Ok("Evaluation updated successfully.");
            }

            // Create a new Evaluation
            var evaluation = new Evaluation
            {
                ParticipantId = participantId,
                FormationId = model.FormationId,
                Rating = model.Rating,
                Review = model.Review,
                DateTime = DateTime.UtcNow
            };
            _context.Evaluations.Add(evaluation);
            await _context.SaveChangesAsync();

            return Ok();

        }

        [HttpGet("{idFormation}")]
        public async Task<IActionResult> GetEvaluation(int idFormation)
        {

   
            // Retrieve all evaluations for the specified formation
            var evaluations = await _context.Evaluations
                .Include(e => e.Participant) // Include Participant details
                .Where(e => e.FormationId == idFormation) // Filter by FormationId
                .ToListAsync();

            if (evaluations == null || !evaluations.Any())
            {
                return NotFound(new { Message = "No evaluations found for this formation." });
            }

            // Map evaluations to EvaluationDTO
            var evaluationDtos = evaluations.Select(e => new EvaluationDTO
            {
                EvaluationId = e.Id,
                ParticipantName = e.Participant.UserName,
                imageUrl= e.Participant.PictureUrl,
                Rating = e.Rating,
                Review = e.Review
            }).ToList();

            return Ok(evaluationDtos);
        }
    }
}