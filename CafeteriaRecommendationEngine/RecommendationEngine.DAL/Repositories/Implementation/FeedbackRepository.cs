using Microsoft.EntityFrameworkCore;
using RecommendationEngine.DAL.DbConnection;
using RecommendationEngine.DAL.Repositories.Interfaces;
using RecommendationEngine.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.DAL.Repositories.Implementation
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly RecommendationEngineContext _context;

        public FeedbackRepository(RecommendationEngineContext context)
        {
            _context = context;
        }

        public async Task<Feedback> GetFeedbackAsync(int userId, int itemId)
        {
            return await _context.Feedback
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ItemId == itemId);
        }

        public async Task AddAsync(Feedback feedback)
        {
            _context.Feedback.Add(feedback);
            await _context.SaveChangesAsync();
        }
    }

}
