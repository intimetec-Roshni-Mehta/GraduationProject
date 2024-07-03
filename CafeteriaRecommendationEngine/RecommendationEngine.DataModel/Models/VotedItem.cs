using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.DataModel.Models
{
    public class VotedItem
    {
        [Key]
        public int VoteId { get; set; }

        // Foreign keys
        [ForeignKey("Users")]
        public int UserId { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }

        [Required]
        public DateTime VoteDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Item Item { get; set; }
    }
}
