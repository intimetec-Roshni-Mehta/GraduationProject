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
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Feedback> Feedback { get; set; }
        public virtual ICollection<VotedItem> VotedItem { get; set; }
        public virtual ICollection<Notification> Notification { get; set; }
    }
}
