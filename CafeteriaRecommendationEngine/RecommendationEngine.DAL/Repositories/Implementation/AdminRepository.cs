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
    public class AdminRepository : IAdminRepository
    {
        private readonly RecommendationEngineContext _context;

        public AdminRepository(RecommendationEngineContext context)
        {
            _context = context;
        }

        public async Task<Item> AddItem(Item newItem)
        {
            _context.Item.Add(newItem);
            await _context.SaveChangesAsync();
            return newItem;
        }

        public async Task<Item> UpdateItem(int itemId, Item updatedItem)
        {
            var existingItem = await _context.Item.FindAsync(itemId);
            if (existingItem != null)
            {
                existingItem.ItemName = updatedItem.ItemName;
                existingItem.Price = updatedItem.Price;
                existingItem.AvailabilityStatus = updatedItem.AvailabilityStatus;
                existingItem.MealTypeId = updatedItem.MealTypeId;

                await _context.SaveChangesAsync();
            }
            return existingItem;
        }

        public async Task<bool> DeleteItem(int itemId)
        {
            var item = await _context.Item.FindAsync(itemId);
            if (item != null)
            {
                _context.Item.Remove(item);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Item>> GetAllItems()
        {
            return await _context.Item.ToListAsync();
        }

        public async Task<Item> GetItemById(int itemId)
        {
            return await _context.Item.FindAsync(itemId);
        }
    }
}
