namespace Online_training.Server.Models.DTOs
{
    public class Certification
    {
        public int CertificateId { get; set; }
        public string ParticipantId { get; set; }
        public int FormationId { get; set; }
        public string? FormationTitle { get; set; }
        public string? InstructorName { get; set; }
        public DateTime DateIssued { get; set; } = DateTime.UtcNow;
        public string? DownloadUrl { get; set; }
    }
}