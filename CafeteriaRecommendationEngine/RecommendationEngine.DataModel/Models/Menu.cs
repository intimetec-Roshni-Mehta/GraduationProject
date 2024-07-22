using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.DataModel.Models
{
    public class Menu
    {
        [Key]
        public int MenuId { get; set; }

        [Required]
        public string Date { get; set; }

        public virtual ICollection<MenuItem> MenuItems { get; set; }
    }

    public class MenuItem
    {
        [Key]
        public int MenuItemId { get; set; }

        [ForeignKey("Menu")]
        public int MenuId { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }
        public bool IsFinalized { get; set; }
        public DateTime? FinalizedDate { get; set; }
        public virtual Menu Menu { get; set; }
        public virtual Item Item { get; set; }
    }

    public class MenuItemDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string MealType { get; set; }
    }

}
