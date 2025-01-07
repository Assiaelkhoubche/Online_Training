using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Stripe;

namespace Online_training.Server.Models
{
    public class Panier
    {
        public int Id { get; set; }
        public string ParticipantId { get; set; }

        public virtual Participant Participant { get; set; }

        public virtual ICollection<PanierItem> PanierItems { get; set; } = new List<PanierItem>();

        [NotMapped]
        public decimal TotalAmount => PanierItems?.Sum(item =>
            item.UnitPrice - (item.DiscountAmount ?? 0)) ?? 0;

        [NotMapped]
        public int TotalItems => PanierItems?.Count ?? 0;
        public string? PaymentIntentId { get; set; }

        public string? participantSecret { get; set; }

        public void AddCourseItem(Formation formation)
        {
            if (PanierItems.All(item => item.FormationId != formation.Id))
            {
                PanierItems.Add(new PanierItem { Formation = formation });
            }
        }
        //public void RemoveCourseItem(int courseId)
        //{
        //    var course = PanierItems.FirstOrDefault(item => item.FormationId == courseId)!;
        //    PanierItems.Remove(course);
        //}
        public void RemoveCourseItem(int courseId)
        {
            var itemToRemove = PanierItems.FirstOrDefault(item => item.FormationId == courseId);
            if (itemToRemove != null)
            {
                PanierItems.Remove(itemToRemove);
            }
        }
        public void RemoveItemsByCondition(Func<PanierItem, bool> condition)
        {
            var itemsToRemove = PanierItems.Where(condition).ToList();
            foreach (var item in itemsToRemove)
            {
                PanierItems.Remove(item);
            }
        }
        public void ClearBasket()
        {
            PaymentIntentId = null;
            participantSecret = null;
            PanierItems.Clear();
        }
        

    }
}
