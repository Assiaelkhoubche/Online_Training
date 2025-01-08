using System.Text.Json.Serialization;

namespace Online_training.Server.Models
{
    public class Formation
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageFormation { get; set; }
        public string Description { get; set; }
        public string? Level { get; set; }
        public string?Language { get; set; }
        public int sutudent { get; set; } = 12;
        public decimal Price { get; set; }
        public decimal? oldPrice { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public string TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        [JsonIgnore]
        public ICollection<Section> Sections { get; set; }
        [JsonIgnore]
        public virtual ICollection<ParticipantFormation>? ParticipantFormations { get; set; }
        [JsonIgnore]
        public virtual ICollection<Evaluation>? Evaluations { get; set; } = new List<Evaluation>();

        public decimal Rating
        {
            get
            {
                if (Evaluations == null || !Evaluations.Any())
                {
                    return 0; // Default rating if no evaluations exist
                }

                // Calculate the average rating
                return Evaluations.Average(e => e.Rating);
            }
        }
        public int students
        {
            get
            {
                if (ParticipantFormations == null || !ParticipantFormations.Any())
                {
                    return 0; // Default rating if no evaluations exist
                }

                // Calculate the average rating
                return ParticipantFormations.Count();
            }
        }


    }
}
