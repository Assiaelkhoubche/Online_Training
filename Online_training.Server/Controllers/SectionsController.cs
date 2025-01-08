using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;
using Online_training.Server.Models;
using System.Security.Claims;
using Online_training.Server.Models.DTOs;

namespace Online_training.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("complte_section")]
        public async Task<IActionResult> CompleteLesson([FromBody] CompleteSectionDTO model)
        {
            var participantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sectionId = model.SectionId;

            var section = await _context.Sections
                .Include(s => s.Formation)
                .FirstOrDefaultAsync(s => s.Id == sectionId);

            if (section == null)
                return NotFound("Section not found");

            var formationId = section.FormationId;

            // Check if the participant is enrolled in the formation
            var participantFormation = await _context.ParticipantFormations
                .FirstOrDefaultAsync(pf => pf.ParticipantId == participantId && pf.FormationId == formationId);

            if (participantFormation == null)
            {
                return NotFound("Participant is not enrolled in this formation.");
            }

            // Check if the lesson is already completed by the participant
            var existingCompletion = await _context.SectionsCompletions
                .FirstOrDefaultAsync(pl => pl.ParticipantId == participantId && pl.SectionId == sectionId && pl.FormationId == formationId);

            if (existingCompletion != null)
            {
                return Conflict("This lesson has already been completed by the participant.");
            }

            // Create a new ParticipantLesson record
            var participantLesson = new SectionCompletion
            {
                ParticipantId = participantId,
                SectionId = sectionId,
                FormationId = formationId,
                CompletedDate = DateTime.UtcNow
            };


            _context.SectionsCompletions.Add(participantLesson);
            await _context.SaveChangesAsync();

            // Calculate progress
            var totalLessons = await _context.Sections
                .CountAsync(s => s.FormationId == formationId);

            var completedLessons = await _context.SectionsCompletions
                .CountAsync(pl => pl.ParticipantId == participantId && pl.FormationId == formationId);

            var progress = (double)completedLessons / totalLessons * 100;
            // Update progress in ParticipantFormation
            participantFormation.Progress = progress;
            _context.ParticipantFormations.Update(participantFormation);
            await _context.SaveChangesAsync();

            return Ok("Lesson marked as completed successfully.");
        }

        [HttpPost("completedsections")]
        public async Task<IActionResult> GetCompletedLessons([FromBody] FormationIdDTO formationIdDTO)
        {
            if (formationIdDTO == null || formationIdDTO.FormationId <= 0)
            {
                return BadRequest("Invalid Formation ID.");
            }

            var participantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Fetch completed lessons for the participant
            var completedLessons = await _context.SectionsCompletions
                .Where(pl => pl.ParticipantId == participantId && pl.FormationId == formationIdDTO.FormationId)
                .Include(pl => pl.Section)
                .Include(pl => pl.Formation)
                .Select(pl => new CompletedSectionDTO
                {
                    SectionId = pl.SectionId,
                    SectionTitle = pl.Section.Title,
                    FormationTitle = pl.Formation.Title,
                    CompletedDate = pl.CompletedDate
                })
                .ToListAsync();

            return Ok(completedLessons);
        }


    }
}