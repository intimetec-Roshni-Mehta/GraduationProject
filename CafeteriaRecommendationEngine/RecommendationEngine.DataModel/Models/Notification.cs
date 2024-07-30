using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.DataModel.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public string NotificationType { get; set; }

        public string Message { get; set; }

        // Foreign key
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
    }
}
