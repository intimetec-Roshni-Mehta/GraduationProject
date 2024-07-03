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
    public class ItemRepository : IItemRepository
    {
        private readonly RecommendationEngineContext _context;

        public ItemRepository(RecommendationEngineContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Item item)
        {
            await _context.Item.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Item item)
        {
            _context.Item.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int itemId)
        {
            var item = await _context.Item.FindAsync(itemId);
            if (item != null)
            {
                _context.Item.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Item> GetByIdAsync(int itemId)
        {
            return await _context.Item.FindAsync(itemId);
        }

        public async Task<List<Item>> GetAllAsync()
        {
            return await _context.Item.ToListAsync();
        }
    }
}
