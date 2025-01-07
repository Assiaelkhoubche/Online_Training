namespace Online_training.Server.Models.DTOs
{
    public class PanierDTO
    {
        public string? ClientId { get; set; }
        public List<PanierItemDTO>? Items { get; set; }

        public string? PaymentIntentId { get; set; }

        public string? ClientSecret { get; set; }
    }
}
