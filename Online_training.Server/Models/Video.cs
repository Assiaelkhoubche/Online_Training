namespace Online_training.Server.Models
{
    public class Video
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string ThumbnailLink { get; set; }
        public TimeSpan Duration { get; set; }
        public int SectionId { get; set; }
        public Section Section { get; set; }
    }
}
