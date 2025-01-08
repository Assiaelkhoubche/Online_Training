namespace Online_training.Server.Certificate_Generation
{
    using iText.IO.Font.Constants;
    using iText.Kernel.Font;
    using iText.Kernel.Pdf;
    using iText.Layout;
    using iText.Layout.Element;
    using iText.Layout.Properties;
    using Online_training.Server.Models.DTOs;
    using System.IO;
    using System.Reflection.PortableExecutable;

    public class CertificateGenerator
    {
        private readonly string _templatePath;

        public CertificateGenerator(string templatePath)
        {
            _templatePath = templatePath;
        }

        public byte[] GenerateCertificate(CertificateData data)
        {
            // Create a temporary output stream for the modified PDF
            using (var memoryStream = new MemoryStream())
            {
                // Open the existing template
                using (var reader = new PdfReader(_templatePath))
                {
                    // Create a new PDF writer
                    var writer = new PdfWriter(memoryStream);

                    // Create a new PDF document using both reader and writer
                    var pdf = new PdfDocument(reader, writer);
                    var document = new Document(pdf);

                    // Set up fonts
                    var titleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    var contentFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                    // Replace participant name
                    // Position values are based on your template - you may need to adjust these
                    var participantName = new Paragraph(data.ParticipantName)
                        .SetFont(titleFont)
                        .SetFontSize(28)
                        .SetFixedPosition(150, 430, 500)  // Adjust x, y, width as needed
                        .SetTextAlignment(TextAlignment.CENTER);
                    document.Add(participantName);

                    // Add course title
                    var courseTitle = new Paragraph(data.CourseTitle)
                        .SetFont(contentFont)
                        .SetFontSize(20)
                        .SetFixedPosition(150, 300, 500)  // Adjust positions as needed
                        .SetTextAlignment(TextAlignment.CENTER);
                    document.Add(courseTitle);

                    // Add instructor name
                    var instructorName = new Paragraph(data.InstructorName)
                        .SetFont(contentFont)
                        .SetFontSize(16)
                        .SetFixedPosition(150, 200, 500)  // Adjust positions as needed
                        .SetTextAlignment(TextAlignment.CENTER);
                    document.Add(instructorName);

                    document.Close();
                    return memoryStream.ToArray();
                }
            }
        }
    }
}