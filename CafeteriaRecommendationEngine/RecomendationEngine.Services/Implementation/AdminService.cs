using Microsoft.EntityFrameworkCore;
using RecomendationEngine.Services.Interfaces;
using RecommendationEngine.DAL.DbConnection;
using RecommendationEngine.DAL.Repositories.Interfaces;
using RecommendationEngine.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomendationEngine.Services.Implementation
{
    public class AdminService : IAdminService
    {
        private readonly IItemRepository _itemRepository;

        public AdminService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task AddItem(string itemName, decimal price, string availabilityStatus, int mealTypeId)
        {
            var item = new Item { ItemName = itemName, Price = price, AvailabilityStatus = availabilityStatus, MealTypeId = mealTypeId };
            await _itemRepository.AddAsync(item);
        }

        public async Task UpdateItem(int itemId, decimal price, string availabilityStatus)
        {
            var item = await _itemRepository.GetByIdAsync(itemId);
            if (item != null)
            {
                item.Price = price;
                item.AvailabilityStatus = availabilityStatus;
                await _itemRepository.UpdateAsync(item);
            }
        }

        public async Task DeleteItem(int itemId)
        {
            await _itemRepository.DeleteAsync(itemId);
        }

        public async Task<List<Item>> GetItems()
        {
            return await _itemRepository.GetAllAsync();
        }
    }
}