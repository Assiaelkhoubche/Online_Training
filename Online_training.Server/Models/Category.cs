namespace Online_training.Server.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Formation>? Formations { get; set; }
    }
}
