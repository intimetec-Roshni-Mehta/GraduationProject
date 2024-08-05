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
using VaderSharp;

namespace RecomendationEngine.Services.Implementation
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IVotedItemRepository _votedItemRepository;
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IAuthService _authService;
        private readonly IChefService _chefService;
        private readonly IMenuRepository _menuRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IVotedItemRepository votedItemRepository, IRecommendationRepository recommendationRepository, IFeedbackRepository feedbackRepository, IAuthService authService, IChefService chefService, IMenuRepository menuRepository, IEmployeeRepository employeeRepository)
        {
            _votedItemRepository = votedItemRepository;
            _recommendationRepository = recommendationRepository;
            _feedbackRepository = feedbackRepository;
            _authService = authService;
            _chefService = chefService;
            _menuRepository = menuRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<string> VoteForItems(int userId, List<int> itemIds)
        {
            var today = DateTime.Now.Date;
            var responseMessages = new List<string>();

            foreach (var itemId in itemIds)
            {
                // Check if the user has already voted for this item today
                var existingVote = await _votedItemRepository.GetVoteAsync(userId, itemId, today);
                if (existingVote != null)
                {
                    responseMessages.Add($"You have already voted for item ID {itemId} today.");
                    continue;
                }

                // Add new vote
                var vote = new VotedItem
                {
                    UserId = userId,
                    ItemId = itemId,
                    VoteDate = today
                };
                await _votedItemRepository.AddAsync(vote);

                // Update recommendation voting count for today
                var recommendation = await _recommendationRepository.GetRecommendationAsync(itemId, today);
                if (recommendation != null)
                {
                    recommendation.Voting++;
                }
                else
                {
                    recommendation = new Recommendation
                    {
                        ItemId = itemId,
                        RecommendedDate = today,
                        Voting = 1
                    };
                    await _recommendationRepository.AddOrUpdateAsync(recommendation);
                }

                // Ensure the item is in the rolled-out menu for today
                await _menuRepository.AddMenuItemAsync(itemId, today.ToString("yyyy-MM-dd"));

                responseMessages.Add($"Vote recorded successfully for item ID {itemId}.");
            }

            await _recommendationRepository.SaveChangesAsync();
            return string.Join("\n", responseMessages);
        }

        public async Task<string> GiveFeedback(int userId, int itemId, int rating, string comment)
        {
            var today = DateTime.Now.Date;
            var finalizedMenu = await _menuRepository.GetFinalizedMenuAsync(today);

            // Ensure finalizedMenuItems is a collection
            if (finalizedMenu == null || !finalizedMenu.MenuItems.Any())
            {
                return "No finalized menu items found for today.";
            }

            // Ensure finalizedMenuItems is a collection of objects with ItemId
            var finalizedItemIds = finalizedMenu.MenuItems
                .Select(menuItem => menuItem.ItemId)  // Ensure menuItem has ItemId
                .ToHashSet();

            if (!finalizedItemIds.Contains(itemId))
            {
                return "Cannot give feedback for an item that is not in the finalized menu.";
            }

            var existingFeedback = await _feedbackRepository.GetFeedbackAsync(userId, itemId);
            if (existingFeedback != null)
            {
                return "You have already given feedback for this item.";
            }

            var feedback = new Feedback
            {
                UserId = userId,
                ItemId = itemId,
                Rating = rating,
                Comment = comment,
                FeedbackDate = DateTime.Now
            };

            await _feedbackRepository.AddAsync(feedback);

            // Assuming sentiment analysis is done here using VaderSharp
            var sentiment = AnalyzeSentiment(comment); // Implement this method

            return $"Feedback recorded successfully. Sentiment: {sentiment}";
        }

        public async Task<string> GetFinalizedMenu(string date)
        {
            var parsedDate = DateTime.Parse(date);

            // Retrieve the finalized menu for the given date
            var menu = await _menuRepository.GetFinalizedMenuAsync(parsedDate);

            if (menu == null)
            {
                return "No menu finalized for the specified date.";
            }

            // Prepare a response string with menu items
            var menuItems = menu.MenuItems.Select(mi => new MenuItemDto
            {
                ID = mi.ItemId,
                Name = mi.Item.ItemName,
                Price = mi.Item.Price,
                MealType = mi.Item.MealType?.MealTypeName ?? "Unknown"
            }).ToList();

            var tableString = ConsoleTableBuilder
                .From(menuItems)
                .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                .Export()
                .ToString();


            return $"Finalized menu for {date}:\n{tableString}";
        }

        public async Task<string> UpdateProfile(string username, string dietPreference, string spiceLevel, string cuisinePreference, string sweetTooth)
        {
            // Fetch user based on username
            var user = await _authService.GetUserIdByUsername(username);
            if (user == null)
            {
                return "User not found";
            }

            // Update user profile information
            var profileUpdateResult = await _employeeRepository.UpdateProfile(user, dietPreference, spiceLevel, cuisinePreference, sweetTooth);

            if (profileUpdateResult)
            {
                return "Profile updated successfully";
            }
            else
            {
                return "Failed to update profile";
            }
        }


        private string AnalyzeSentiment(string text)
        {
            var analyzer = new SentimentIntensityAnalyzer();
            var results = analyzer.PolarityScores(text);
            if (results.Compound > 0.05)
            {
                return "Positive";
            }
            else if (results.Compound < -0.05)
            {
                return "Negative";
            }
            else
            {
                return "Neutral";
            }
        }
    }
}