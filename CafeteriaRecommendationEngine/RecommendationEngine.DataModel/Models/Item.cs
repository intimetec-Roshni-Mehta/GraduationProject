using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.DataModel.Models
{
    public class Item
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        public string ItemName { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required]
        public string AvailabilityStatus { get; set; }

        [ForeignKey("MealType")]
        public int MealTypeId { get; set; }


        // Navigation properties
        public virtual MealType MealType { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Recommendation> Recommendations { get; set; }
    }
}
