namespace Online_training.Server.Models.DTOs
{
    public class CreateVideoDto
    {
        public int? Id { get; set; }
        public string? Link { get; set; }
        public string? Name { get; set; }
        public string? ThumbnailLink { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
