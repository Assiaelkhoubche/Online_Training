namespace Online_training.Server.Models
{
    public class Trainer:User
    {
        public string Speciality { get; set; }
        public ICollection<Formation>? Formations { get; set; } = new List<Formation>();

    }
}
