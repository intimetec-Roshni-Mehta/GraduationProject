using Microsoft.EntityFrameworkCore;
using RecommendationEngine.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.DAL.DbConnection
{
    public class RecommendationEngineContext : DbContext
    {
        public RecommendationEngineContext(DbContextOptions<RecommendationEngineContext> options)
            : base(options)
        {
        }

        // Define your DbSets here
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Recommendation> Recommendation { get; set; }
        public DbSet<MealType> MealType { get; set; }
        public DbSet<VotedItem> VotedItem { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<MenuItem> MenuItem { get; set; }

    }
}