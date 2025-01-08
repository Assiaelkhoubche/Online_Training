using System.ComponentModel.DataAnnotations;

namespace Online_training.Server.Models
{
    public class Evaluation
    {
        [Key]
        public int Id { get; set; }
        public string ParticipantId { get; set; }
        public Participant Participant { get; set; }
        public int FormationId { get; set; }
        public Formation Formation { get; set; }
        public decimal Rating { get; set; }
        public string Review { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }
}
