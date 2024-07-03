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
        Task<Recommendation> GetRecommendationAsync(int itemId);
        Task AddOrUpdateAsync(Recommendation recommendation);
        Task SaveChangesAsync();
    }
}
