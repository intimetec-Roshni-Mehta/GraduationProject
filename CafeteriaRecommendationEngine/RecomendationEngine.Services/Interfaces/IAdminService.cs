using RecommendationEngine.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomendationEngine.Services.Interfaces
{
    public interface IAdminService
    {
        Task AddItem(string itemName, decimal price, string availabilityStatus, int mealTypeId);
        Task UpdateItem(int itemId, decimal price, string availabilityStatus);
        Task DeleteItem(int itemId);
        Task<List<Item>> GetItems();
    }

}
