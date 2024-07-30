using RecommendationEngine.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.DAL.Repositories.Interfaces
{
    public interface IVotedItemRepository
    {
        Task<VotedItem> GetVoteAsync(int userId, int itemId, DateTime voteDate);
        Task AddAsync(VotedItem vote);
    }
}
