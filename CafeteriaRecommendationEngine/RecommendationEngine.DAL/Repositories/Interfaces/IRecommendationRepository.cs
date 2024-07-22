using RecommendationEngine.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.DAL.Repositories.Interfaces
{
    public interface IRecommendationRepository
    {
        Task<Recommendation> GetRecommendationAsync(int itemId, DateTime date);
        Task AddOrUpdateAsync(Recommendation recommendation);
        Task SaveChangesAsync();
        Task<List<Item>> GetVotedItemsAsync(DateTime date);
    }
}
