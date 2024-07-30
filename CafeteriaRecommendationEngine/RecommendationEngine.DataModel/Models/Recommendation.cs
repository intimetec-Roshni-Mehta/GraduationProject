using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.DataModel.Models
{
    public class Recommendation
    {
        [Key]
        public int RecommendationId { get; set; }

        // Foreign key
        [ForeignKey("Item")]
        public int ItemId { get; set; }

        [Required]
        public DateTime RecommendedDate { get; set; }

        public int Voting {  get; set; }

        // Navigation properties
        public virtual Item Item { get; set; }
    }
}
