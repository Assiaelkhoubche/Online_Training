namespace Online_training.Server.Models.DTOs
{
    public class CreateSectionDto
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public int OrderIndex { get; set; }
        public bool IsPreview { get; set; }
        public List<CreateVideoDto> Videos { get; set; }
    }
}
