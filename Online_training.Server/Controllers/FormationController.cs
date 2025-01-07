using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;
using Online_training.Server.Models;
using Online_training.Server.Models.DTOs;
using System.Security.Claims;
using Humanizer;

namespace Online_training.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FormationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Formation

        [HttpGet]
        public async Task<ActionResult<Formation>> GetAllFormation()
        {
            var allFormations= _context.Formations
                                        .Include(f=>f.Category)
                                        .Include(f=>f.Sections)
                                        .ThenInclude(s=>s.Videos)
                                        .ToList();
            if (allFormations == null)
            {
                return BadRequest(new { message = "No formation Founded" });
            }
            return Ok(allFormations);
            
        }

        [HttpGet("trainer")]
        public async Task<ActionResult<Formation>> GetFormationsByTrainer()
        {
         
            var trainerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (trainerId == null)
            {
                return Unauthorized("Trainer is not authenticated");
            }


            var formations = await _context.Formations
                                           .Include(f => f.Category)
                                           .Include(f => f.Sections)
                                               .ThenInclude(s => s.Videos)
                                           .Where(f => f.TrainerId.ToString() == trainerId)
                                           .ToListAsync();

            if (formations == null || formations.Count == 0)
            {
                return NotFound(new { message = "No formations found for the authenticated trainer" });
            }

            return Ok(formations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CreateFormationDto>> GetFormation(int id)
        {
            // Fetch formation with related data
            var formation = _context.Formations
                                    .Include(f => f.Category)
                                    .Include(f => f.Sections)
                                    .ThenInclude(s => s.Videos)
                                    .FirstOrDefault(f => f.Id == id);

            if (formation == null)
            {
                return BadRequest(new { message = "No formation found with these credentials" });
            }

            // Map formation data to DTO
            var newFormation = new CreateFormationDto
            {
                Title = formation.Title,
                Description = formation.Description,
                ImageFormation = formation.ImageFormation,
                Level = formation.Level,
                Language = formation.Language,
                Price = formation.Price,
                OldPrice = formation.oldPrice,
                Sutudent = formation.sutudent,
                CategoryId = formation.CategoryId,
                TrainerId = formation.TrainerId,
                Category = new CategoryDto
                {
                    Id = formation.Category?.Id ?? 0,
                    Name = formation.Category?.Name
                },
                Sections = new List<CreateSectionDto>()
            };

            // Map sections and videos
            foreach (var section in formation.Sections)
            {
                var newSection = new CreateSectionDto
                {
                    Title = section.Title,
                    OrderIndex = section.OrderIndex,
                    IsPreview = section.IsPreview,
                    Videos = new List<CreateVideoDto>()
                };

                foreach (var video in section.Videos)
                {
                    var newVideo = new CreateVideoDto
                    {
                        Name = video.Name,
                        Link = video.Link,
                        ThumbnailLink = video.ThumbnailLink,
                        Duration = video.Duration // Example formatting
                    };

                    newSection.Videos.Add(newVideo);
                }

                newFormation.Sections.Add(newSection);
            }

            return Ok(newFormation);
        }

        //POST
        [HttpPost]
       
        public async Task<ActionResult<Formation>> CreateFormation(CreateFormationDto dto)
        {
            var trainerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (trainerId == null)
            {
                return Unauthorized("Trainer is not authenticated");
            }

            // Create formation
            var formation = new Formation
            {
                Title = dto.Title,
                Description = dto.Description,
                TrainerId = trainerId,
                ImageFormation = dto.ImageFormation,
                Level = dto.Level,
                Language = dto.Language,
                Price = dto.Price,
                oldPrice = dto.OldPrice,
                sutudent = dto.Sutudent,
                CategoryId = dto.CategoryId, // Assuming you're handling categoryId on the backend
                Sections = dto.Sections.Select(s => new Section
                {
                    Title = s.Title,
                    OrderIndex = s.OrderIndex,
                    IsPreview = s.IsPreview,
                    Videos = s.Videos.Select(v => new Video
                    {
                        Name = v.Name,
                        Link = v.Link,
                        ThumbnailLink = v.ThumbnailLink,
                        Duration = v.Duration
                    }).ToList()
                }).ToList()
            };

            // You would typically add this formation to the database and save it.
            _context.Formations.Add(formation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateFormation), new { id = formation.Id }, formation);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<Formation>> UpdateFormation(int id, CreateFormationDto dto)
        {
            // Find the formation by id
            var formation = await _context.Formations
                                          .Include(f => f.Category)
                                          .Include(f => f.Sections)
                                          .ThenInclude(s => s.Videos)
                                          .FirstOrDefaultAsync(f => f.Id == id);

            if (formation == null)
            {
                return NotFound(new { message = "Formation not found" });
            }

            // Check if the trainer is authorized to update the formation
            var trainerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (trainerId == null || formation.TrainerId.ToString() != trainerId)
            {
                return Unauthorized("Trainer is not authorized to update this formation");
            }

            // Update the formation's properties
            formation.Title = dto.Title ?? formation.Title;
            formation.Description = dto.Description ?? formation.Description;
            formation.ImageFormation = dto.ImageFormation ?? formation.ImageFormation;
            formation.Level = dto.Level ?? formation.Level;
            formation.Language = dto.Language ?? formation.Language;
            formation.Price = dto.Price;
            formation.oldPrice = dto.OldPrice ?? formation.oldPrice;
            formation.sutudent = dto.Sutudent;
            formation.CategoryId = dto.CategoryId;

            // Update sections and videos if provided
            if (dto.Sections != null)
            {
                foreach (var sectionDto in dto.Sections)
                {
                    var section = formation.Sections.FirstOrDefault(s => s.Id == sectionDto.Id);
                    if (section != null)
                    {
                        // Update section details
                        section.Title = sectionDto.Title ?? section.Title;
                        section.OrderIndex = sectionDto.OrderIndex ;
                        section.IsPreview = sectionDto.IsPreview;

                        // Update videos if provided
                        if (sectionDto.Videos != null)
                        {
                            foreach (var videoDto in sectionDto.Videos)
                            {
                                var video = section.Videos.FirstOrDefault(v => v.Id == videoDto.Id);
                                if (video != null)
                                {
                                    video.Name = videoDto.Name ?? video.Name;
                                    video.Link = videoDto.Link ?? video.Link;
                                    video.ThumbnailLink = videoDto.ThumbnailLink ?? video.ThumbnailLink;
                                    video.Duration = videoDto.Duration;
                                }
                            }
                        }
                    }
                }
            }

            // Save the changes to the database
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the formation", error = ex.Message });
            }

            return Ok(formation);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFormation(int id)
        {
            // Find the formation by id
            var formation = await _context.Formations
                                          .Include(f => f.Sections)
                                          .ThenInclude(s => s.Videos)
                                          .FirstOrDefaultAsync(f => f.Id == id);

            if (formation == null)
            {
                return NotFound(new { message = "Formation not found" });
            }

            // Check if the trainer is authorized to delete the formation
            var trainerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (trainerId == null || formation.TrainerId.ToString() != trainerId)
            {
                return Unauthorized(new { message = "You are not authorized to delete this formation" });
            }

            // Remove the formation from the database
            _context.Formations.Remove(formation);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Formation deleted successfully" });
        }

    }
}