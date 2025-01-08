using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_training.Server.Models;
using Online_training.Server.Models.DTOs;

namespace Online_training.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantFormationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ParticipantFormationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<ParticipantFormationDto>>> GetFormationsByParticipantId()
        {
            var participantId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Fetch participant formations
            var participantFormations = await _context.ParticipantFormations
                .Where(pf => pf.ParticipantId == participantId)
                .Include(pf => pf.Formation)
                .ThenInclude(f => f.Category)
                .Select(pf => new ParticipantFormationDto
                {
                    formationId = pf.FormationId,
                    Title = pf.Formation.Title,
                    CategoryName = pf.Formation.Category.Name,
                    PurchasedOn = pf.EnrollmentDate,
                    ImageFormation = pf.Formation.ImageFormation,
                    Progress = pf.Progress ?? 0,
                    CreatorName = pf.Formation.TrainerId != null ? null : "No Trainer Assigned"
                })
                .ToListAsync();

            // Populate trainer names
            foreach (var formation in participantFormations)
            {
                if (formation.CreatorName == null)
                {
                    var trainerName = await _context.Participants
                        .Where(p => p.Id == _context.Formations
                            .Where(f => f.Title == formation.Title)
                            .Select(f => f.TrainerId)
                            .FirstOrDefault())
                        .Select(p => p.UserName)
                        .FirstOrDefaultAsync();
                    formation.CreatorName = trainerName ?? "Trainer Not Found";
                }
            }

            if (participantFormations == null || participantFormations.Count == 0)
            {
                return NotFound("No formations found for this participant.");
            }

            return Ok(participantFormations);
        }

    }
}
