using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Online_training.Server.Models
{
    public class PanierItem
    {
        public int Id { get; set; }

     
        public int PanierId { get; set; }
        
        [JsonIgnore]
        public virtual Panier Panier { get; set; }

        public int FormationId { get; set; }

        public virtual Formation Formation { get; set; }

       
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountAmount { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
}
