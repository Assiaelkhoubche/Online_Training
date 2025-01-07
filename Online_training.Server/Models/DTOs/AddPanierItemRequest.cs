namespace Online_training.Server.Models.DTOs
{
    public class AddPanierItemRequest
    {
        public int FormationId { get; set; }
        //public string participantId { get; set; }
        
        public decimal? DiscountAmount { get; set; }

    }
}
