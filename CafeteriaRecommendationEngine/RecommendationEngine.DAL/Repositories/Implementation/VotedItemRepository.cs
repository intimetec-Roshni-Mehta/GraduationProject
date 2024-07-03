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
    public class VotedItemRepository : IVotedItemRepository
    {
        private readonly RecommendationEngineContext _context;

        public VotedItemRepository(RecommendationEngineContext context)
        {
            _context = context;
        }

        public async Task<VotedItem> GetVoteAsync(int userId, int itemId, DateTime voteDate)
        {
            return await _context.VotedItem
                .FirstOrDefaultAsync(v => v.UserId == userId && v.ItemId == itemId && v.VoteDate == voteDate);
        }

        public async Task AddAsync(VotedItem vote)
        {
            _context.VotedItem.Add(vote);
            await _context.SaveChangesAsync();
        }
    }
}
