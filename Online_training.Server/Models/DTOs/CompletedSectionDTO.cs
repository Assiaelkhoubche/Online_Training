namespace Online_training.Server.Models.DTOs
{
    public class CompletedSectionDTO
    {
        public int SectionId { get; set; }
        public string SectionTitle { get; set; }
        public string FormationTitle { get; set; }
        public string? Image { get; set; }
        public DateTime CompletedDate { get; set; }
    }
}