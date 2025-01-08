namespace Online_training.Server.Models.DTOs
{
    public class ParticipantFormationDto
    {
        public string? Title { get; set; }
        public string? CategoryName { get; set; }
        public int? formationId { get; set; }
        public DateTime? PurchasedOn { get; set; }
        public string? ImageFormation { get; set; }
        public string? CreatorName { get; set; }
        public double? Progress { get; set; }
    }
}
