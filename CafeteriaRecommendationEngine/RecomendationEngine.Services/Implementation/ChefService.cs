using Microsoft.EntityFrameworkCore;
using RecomendationEngine.Services.Interfaces;
using RecommendationEngine.DAL.Repositories.Interfaces;
using RecommendationEngine.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomendationEngine.Services.Implementation
{
    public class ChefService : IChefService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IItemRepository _itemRepository;

        public ChefService(IMenuRepository menuRepository, IItemRepository itemRepository)
        {
            _menuRepository = menuRepository;
            _itemRepository = itemRepository;
        }

        public async Task<string> RolloutMenu(string date, List<int> itemIds)
        {
            // Check if a menu already exists for the given date
            var existingMenu = await _menuRepository.GetByDateAsync(date);
            if (existingMenu.Count != 0)
            {
                return "Menu has already been rolled out for this date.";
            }

            // Create and save the new menu
            var newMenu = new Menu
            {
                Date = date,
                MenuItems = itemIds.Select(id => new MenuItem { ItemId = id }).ToList()
            };

            await _menuRepository.AddAsync(newMenu);
            return "Menu rolled out successfully for tomorrow";
        }

        public async Task<bool> CheckMenuRolledOut(string date)
        {
            var menus = await _menuRepository.GetByDateAsync(date);
            return menus.Any();
        }

        public async Task<List<Item>> GetRolledOutMenu(string date)
        {
            var menus = await _menuRepository.GetByDateWithItemsAndRecommendationsAsync(date);
            if (menus == null || menus.Count == 0)
            {
                return new List<Item>();
            }

            return menus.SelectMany(menu => menu.MenuItems.Select(mi => mi.Item)).ToList();
        }

    }

}