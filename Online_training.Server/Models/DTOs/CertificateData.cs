namespace Online_training.Server.Models.DTOs
{
    public class CertificateData
    {
        // The full name of the person receiving the certificate
        public string ParticipantName { get; set; }

        // The title of the course or training program completed
        public string CourseTitle { get; set; }

        // The name of the instructor who taught the course
        public string InstructorName { get; set; }

        // The date when the certificate was issued
        public DateTime DateIssued { get; set; }

        // Constructor to make it easier to create new certificate data
        public CertificateData(string participantName, string courseTitle, string instructorName, DateTime dateIssued)
        {
            ParticipantName = participantName;
            CourseTitle = courseTitle;
            InstructorName = instructorName;
            DateIssued = dateIssued;
        }

        // Default constructor for when you need to set properties individually
        public CertificateData() { }
    }
}