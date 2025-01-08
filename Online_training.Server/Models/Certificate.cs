using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Online_training.Server.Models
{
    public class Certificate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string ParticipantId { get; set; }
        [Required]
        public int FormationId { get; set; }

        [ForeignKey("ParticipantId")]
        public virtual Participant Participant { get; set; }
        [DeleteBehavior(DeleteBehavior.NoAction)]
        [ForeignKey("FormationId")]
        public virtual Formation Formation { get; set; }
        public DateTime DateIssued { get; set; }
        public string? FilePath { get; set; }
        public string? DownloadUrl { get; set; }
    }
}
