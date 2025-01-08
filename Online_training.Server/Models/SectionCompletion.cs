using Online_training.Server.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class SectionCompletion
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string ParticipantId { get; set; }

    [ForeignKey("ParticipantId")]
    public virtual Participant Participant { get; set; }

    [Required]
    public int SectionId { get; set; }

    [ForeignKey("SectionId")]
    public virtual Section Section { get; set; }

    [Required]
    public int FormationId { get; set; }

    [ForeignKey("FormationId")]
    public virtual Formation Formation { get; set; }

    public DateTime CompletedDate { get; set; } = DateTime.UtcNow;
}