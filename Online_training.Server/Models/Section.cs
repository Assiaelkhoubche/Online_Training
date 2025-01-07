using System.Text.Json.Serialization;

namespace Online_training.Server.Models
{
    public class Section
    {
        public int Id { get; set; }
        public int OrderIndex { get; set; }
        public bool IsPreview { get; set; } = false;
        public string Title { get; set; }
        public int FormationId { get; set; }
        public Formation Formation { get; set; }

        [JsonIgnore]
        public ICollection<Video> Videos { get; set; }
    }
}
