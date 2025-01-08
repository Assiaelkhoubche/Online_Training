namespace Online_training.Server.Models.DTOs
{
    public class EvaluationDTO
    {
        public int? EvaluationId { get; set; }
        public string? ParticipantId { get; set; }
        public string? imageUrl {  get; set; }
        public string? ParticipantName { get; set; }
        public decimal? Rating { get; set; }
        public string? Review { get; set; }
    }
}
