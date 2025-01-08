using System.Text.Json.Serialization;

namespace Online_training.Server.Models
{
    public class Participant:User
    {
        public bool IsPro { get; set; } = false;
        [JsonIgnore]
        public virtual ICollection<ParticipantFormation>? ParticipantFormations { get; set; }
        [JsonIgnore]
        public virtual ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    }
}
