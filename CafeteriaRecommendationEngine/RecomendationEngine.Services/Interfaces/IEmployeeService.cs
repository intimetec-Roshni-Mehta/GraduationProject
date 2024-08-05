using RecommendationEngine.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomendationEngine.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<string> VoteForItems(int userId, List<int> itemIds);
        Task<string> GiveFeedback(int userId, int itemId, int rating, string comment);
        Task<string> GetFinalizedMenu(string date);
        Task<string> UpdateProfile(string userName, string dietPreference, string spiceLevel, string cuisinePreference, string sweetTooth);
    }
}
