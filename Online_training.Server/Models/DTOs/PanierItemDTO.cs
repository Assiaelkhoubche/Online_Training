namespace Online_training.Server.Models.DTOs
{
    public class PanierItemDTO
    {
        public int CourseId { get; set; }
        public string? Title { get; set; }
        public float Price { get; set; }
        public string? Image { get; set; }
        public string? trainer { get; set; }
    }
}
