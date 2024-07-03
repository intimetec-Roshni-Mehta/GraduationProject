using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.DataModel.Models
{
    public class MealType
    {
        [Key]
        public int MealTypeId { get; set; }

        [Required]
        public string MealTypeName { get; set; }

        // Navigation properties
        public virtual ICollection<Item> Item { get; set; }
    }
}
