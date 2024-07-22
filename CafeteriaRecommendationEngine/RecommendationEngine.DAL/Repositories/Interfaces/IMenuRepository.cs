using RecommendationEngine.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.DAL.Repositories.Interfaces
{
    public interface IMenuRepository
    {
        Task AddAsync(Menu menu);
        Task<List<Menu>> GetByDateAsync(string date);
        Task AddMenuItemAsync(int itemId, string date);
        Task<List<Menu>> GetByDateWithItemsAndRecommendationsAsync(string date);
        Task<bool> IsMenuRolledOutAsync(DateTime date);
        Task<Menu> GetFinalizedMenuAsync(DateTime date);
    }
}