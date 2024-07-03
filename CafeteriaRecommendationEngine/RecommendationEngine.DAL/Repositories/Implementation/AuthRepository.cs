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
    public class AuthRepository : IAuthRepository
    {
        private readonly RecommendationEngineContext _dbContext;

        public AuthRepository(RecommendationEngineContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> Authenticate(string username)
        {
            var user = await _dbContext.User.Include(u => u.Role)
                                            .SingleOrDefaultAsync(u => u.UserName == username);

            return user;
        }

        public async Task<int?> GetUserIdByUsername(string username)
        {
            var user = await _dbContext.User.SingleOrDefaultAsync(u => u.UserName == username);
            return user?.UserId;
        }
    }

}
