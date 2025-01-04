namespace Online_training.Server.Models
{
    public class TrainerRequest
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public User User { get; set; } 
        public string Status { get; set; } = "Pending"; // "Pending", "Approved", or "Rejected"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
