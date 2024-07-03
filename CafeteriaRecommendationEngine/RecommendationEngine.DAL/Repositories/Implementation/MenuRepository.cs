using Microsoft.EntityFrameworkCore;
using RecommendationEngine.DAL.DbConnection;
using RecommendationEngine.DAL.Repositories.Interfaces;
using RecommendationEngine.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecommendationEngine.DAL.Repositories.Implementation
{
    public class MenuRepository : IMenuRepository
    {
        private readonly RecommendationEngineContext _context;

        public MenuRepository(RecommendationEngineContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Menu menu)
        {
            await _context.Menu.AddAsync(menu);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Menu>> GetByDateAsync(string date)
        {
            return await _context.Menu
                .Include(m => m.MenuItems)
                .ThenInclude(mi => mi.Item)
                .ThenInclude(i => i.MealType) // Include MealType
                .Where(m => m.Date == date)
                .ToListAsync();
        }

        public async Task AddMenuItemAsync(int itemId, string date)
        {
            var menu = await _context.Menu
                .Include(m => m.MenuItems)
                .FirstOrDefaultAsync(m => m.Date == date);

            if (menu == null)
            {
                menu = new Menu
                {
                    Date = date,
                    MenuItems = new List<MenuItem> { new MenuItem { ItemId = itemId } }
                };
                await _context.Menu.AddAsync(menu);
            }
            else
            {
                if (!menu.MenuItems.Any(mi => mi.ItemId == itemId))
                {
                    menu.MenuItems.Add(new MenuItem { ItemId = itemId });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Menu>> GetByDateWithItemsAndRecommendationsAsync(string date)
        {
            return await _context.Menu
                .Include(m => m.MenuItems)
                .ThenInclude(mi => mi.Item)
                .ThenInclude(i => i.Recommendations)  // Include Recommendations here
                .Include(m => m.MenuItems)
                .ThenInclude(mi => mi.Item)
                .ThenInclude(i => i.MealType)  // Include MealType here
                .Where(m => m.Date == date)
                .ToListAsync();
        }


    }
}
