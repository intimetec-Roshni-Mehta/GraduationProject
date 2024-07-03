using Microsoft.EntityFrameworkCore;
using RecommendationEngine.DAL.DbConnection;
using RecommendationEngine.DAL.Repositories.Interfaces;
using RecommendationEngine.DataModel.Models;
using System.Threading.Tasks;

namespace RecommendationEngine.DAL.Repositories.Implementation
{
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly RecommendationEngineContext _context;

        public RecommendationRepository(RecommendationEngineContext context)
        {
            _context = context;
        }

        public async Task<Recommendation> GetRecommendationAsync(int itemId)
        {
            return await _context.Recommendation.FirstOrDefaultAsync(r => r.ItemId == itemId);
        }

        public async Task AddOrUpdateAsync(Recommendation recommendation)
        {
            var existingRecommendation = await _context.Recommendation.FirstOrDefaultAsync(r => r.ItemId == recommendation.ItemId);
            if (existingRecommendation != null)
            {
                existingRecommendation.Voting++;
            }
            else
            {
                _context.Recommendation.Add(recommendation);
            }
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
