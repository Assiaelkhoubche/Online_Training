namespace Online_training.Server.Models.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageFormation { get; set; }
        public string Description { get; set; }
        public string? Level { get; set; }
        public string? Language { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public int Sutudent { get; set; }
        public int CategoryId { get; set; }
        public CategoryDto? Category { get; set; }
        public string? TrainerId { get; set; }
        public double?progress { get; set; }
        public List<CreateSectionDto> Sections { get; set; }
    }
}
