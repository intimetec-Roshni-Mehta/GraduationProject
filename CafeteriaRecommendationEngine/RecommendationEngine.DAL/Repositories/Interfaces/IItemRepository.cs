using RecommendationEngine.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.DAL.Repositories.Interfaces
{
    public interface IItemRepository
    {
        Task AddAsync(Item item);
        Task UpdateAsync(Item item);
        Task DeleteAsync(int itemId);
        Task<Item> GetByIdAsync(int itemId);
        Task<List<Item>> GetAllAsync();
    }
}
