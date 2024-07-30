using RecommendationEngine.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.DAL.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<Item> AddItem(Item newItem);
        Task<Item> UpdateItem(int itemId, Item updatedItem);
        Task<bool> DeleteItem(int itemId);
        Task<List<Item>> GetAllItems();
        Task<Item> GetItemById(int itemId);
    }
}

