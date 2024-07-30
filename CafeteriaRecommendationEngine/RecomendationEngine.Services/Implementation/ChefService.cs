using ConsoleTableExt;
using Microsoft.EntityFrameworkCore;
using RecomendationEngine.Services.Interfaces;
using RecommendationEngine.DAL.Repositories.Implementation;
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
        private readonly IRecommendationRepository _recommendationRepository;

        public ChefService(IMenuRepository menuRepository, IItemRepository itemRepository, IRecommendationRepository recommendationRepository)
        {
            _menuRepository = menuRepository;
            _itemRepository = itemRepository;
            _recommendationRepository = recommendationRepository;
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

        public async Task<string> GetVotedItems()
        {
            var today = DateTime.Now.Date;
            var votedItems = await _recommendationRepository.GetVotedItemsAsync(today);

            if (votedItems == null || !votedItems.Any())
            {
                return "No items have been voted on.";
            }

            var itemDetails = votedItems.Select(item => new VotedItemDto
            {
                ID = item.ItemId,
                Name = item.ItemName,
                MealType = item.MealType?.MealTypeName ?? "Unknown",
                Votes = item.Recommendations.FirstOrDefault()?.Voting ?? 0
            }).ToList();

            var tableString = ConsoleTableBuilder
                .From(itemDetails)
                .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                .Export()
                .ToString();

            return $"Voted items:\n{tableString}";
        }

        public async Task<string> FinalizeMenu(string date, List<int> itemIds)
        {
            var parsedDate = DateTime.Parse(date);
            var previousDate = parsedDate.AddDays(-1);

            var isMenuRolledOut = await _menuRepository.IsMenuRolledOutAsync(parsedDate);
            if (!isMenuRolledOut)
            {
                return "Menu has not been rolled out for the specified date.";
            }

            var votedItems = await _recommendationRepository.GetVotedItemsAsync(previousDate);

            if (votedItems == null || !votedItems.Any())
            {
                return "No items have been voted on.";
            }

            var filteredVotedItems = votedItems.Where(item => itemIds.Contains(item.ItemId)).ToList();

            if (!filteredVotedItems.Any())
            {
                return "No voted items match the provided item IDs.";
            }

            var topItems = filteredVotedItems.OrderByDescending(item => item.Recommendations.FirstOrDefault()?.Voting ?? 0)
                                             .Take(itemIds.Count)
                                             .Select(item => item.ItemId)
                                             .ToList();

            if (!topItems.Any())
            {
                return "No items to finalize.";
            }

            var newMenu = new Menu
            {
                Date = parsedDate.ToString("yyyy-MM-dd"),
                MenuItems = topItems.Select(id => new MenuItem { ItemId = id }).ToList()
            };

            await _menuRepository.AddAsync(newMenu);

            return "Menu finalized based on votes.";
        }

    }
}