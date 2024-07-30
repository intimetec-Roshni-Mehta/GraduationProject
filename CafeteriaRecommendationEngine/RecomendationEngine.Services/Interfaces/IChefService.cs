using RecommendationEngine.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomendationEngine.Services.Interfaces
{
    public interface IChefService
    {
        Task<string> RolloutMenu(string date, List<int> itemIds);

        Task<bool> CheckMenuRolledOut(string date);
        Task<List<Item>> GetRolledOutMenu(string date);
        Task<string> GetVotedItems();
        Task<string> FinalizeMenu(string date, List<int> itemIds);
    }
}
