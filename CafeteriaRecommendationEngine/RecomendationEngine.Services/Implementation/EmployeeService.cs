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

        public EmployeeService(IVotedItemRepository votedItemRepository, IRecommendationRepository recommendationRepository, IFeedbackRepository feedbackRepository, IAuthService authService, IChefService chefService, IMenuRepository menuRepository)
        {
            _votedItemRepository = votedItemRepository;
            _recommendationRepository = recommendationRepository;
            _feedbackRepository = feedbackRepository;
            _authService = authService;
            _chefService = chefService;
            _menuRepository = menuRepository;
        }

        public async Task<string> VoteForItems(int userId, List<int> itemIds)
        {
            var today = DateTime.Now.Date;
            var successfulVotes = new List<int>();
            var duplicateVotes = new List<int>();

            foreach (var itemId in itemIds)
            {
                // Check if the user has already voted for this item today
                var existingVote = await _votedItemRepository.GetVoteAsync(userId, itemId, today);
                if (existingVote != null)
                {
                    duplicateVotes.Add(itemId);
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

                // Update recommendation voting count
                var recommendation = await _recommendationRepository.GetRecommendationAsync(itemId);
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

                // Ensure the item is in the rolled-out menu
                await _menuRepository.AddMenuItemAsync(itemId, today.ToString("yyyy-MM-dd"));

                successfulVotes.Add(itemId);
            }

            await _recommendationRepository.SaveChangesAsync();

            var responseMessage = new StringBuilder();
            if (successfulVotes.Any())
            {
                responseMessage.AppendLine($"You have successfully voted for the following item IDs: {string.Join(", ", successfulVotes)}.");
            }
            if (duplicateVotes.Any())
            {
                responseMessage.AppendLine($"You have already voted for the following item IDs today: {string.Join(", ", duplicateVotes)}.");
            }

            return responseMessage.ToString();
        }

        public async Task<string> GiveFeedback(int userId, int itemId, int rating, string comment)
        {
            var rolledOutItems = await _chefService.GetRolledOutMenu(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
            var rolledOutItemIds = rolledOutItems.Select(item => item.ItemId);

            if (!rolledOutItemIds.Contains(itemId))
            {
                return "Cannot give feedback for an item that is not in the rolled-out menu.";
            }

            // Check if the employee has already given feedback for this item
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