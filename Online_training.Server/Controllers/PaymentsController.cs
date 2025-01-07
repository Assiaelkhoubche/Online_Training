
using System.Security.Claims;
using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Online_training.Server.Models;
using Online_training.Server.Models.DTOs;
using Online_training.Server.Services;


namespace Online_training.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class PaymentsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly PaymentService _paymentService;


         

        public PaymentsController(PaymentService paymentService,ApplicationDbContext context, IMapper mapper)
        {
            _paymentService = paymentService;
            _context = context;
            _mapper = mapper;
        }

        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<PanierDTO>> PaymentIntentAsync()
        {

            var clientId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientId))
            {
                return NotFound("Client ID is missing or invalid");
            }
            var basket = await _context.Paniers
                .Include(b => b.PanierItems)
                .ThenInclude(i => i.Formation)
                .FirstOrDefaultAsync(x => x.ParticipantId == clientId);

            if (basket == null)
            {
                // Log the issue
                Console.WriteLine($"No basket found for ParticipantId: {clientId}");
            }

            if (basket == null) return NotFound("No basket found");

            var intent = await _paymentService.PaymentIntentAsync(basket);

            if (intent == null) return BadRequest("Problem creating the payment intent");

            basket.PaymentIntentId = basket.PaymentIntentId ?? intent.Id;
            basket.participantSecret = basket.participantSecret ?? intent.ClientSecret;

            _context.Update(basket);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Problem updating basket with intent");
            return _mapper.Map<Panier, PanierDTO>(basket);

        }
       
        private async Task<Panier> ExtractBasket(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                Response.Cookies.Delete("clientId");
                return null;
            }
            var basket = await _context.Paniers
                        .Include(b => b.PanierItems)
                        .ThenInclude(i => i.Formation)
                        .OrderBy(i => i.Id)
                        .FirstOrDefaultAsync(x => x.ParticipantId == clientId);
            return basket!;

        }


    }


    public class DtoParticipant
    {
        public string Id { get; set; }
    }
}