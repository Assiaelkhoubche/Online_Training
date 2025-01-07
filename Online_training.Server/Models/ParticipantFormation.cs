using System.ComponentModel.DataAnnotations.Schema;

namespace Online_training.Server.Models
{
    public class ParticipantFormation
    {
        public int Id { get; set; }      
        public string ParticipantId { get; set; }       
        public int FormationId { get; set; }      
        public  Participant Participant { get; set; }      
        public Formation Formation { get; set; }
        public double? Progress { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
