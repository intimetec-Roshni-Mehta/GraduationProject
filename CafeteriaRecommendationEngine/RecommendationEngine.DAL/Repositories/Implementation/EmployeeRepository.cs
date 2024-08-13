using RecommendationEngine.DAL.DbConnection;
using RecommendationEngine.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.DAL.Repositories.Implementation
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly RecommendationEngineContext _dbContext;

        public EmployeeRepository(RecommendationEngineContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> UpdateProfile(int? userId, string dietPreference, string spiceLevel, string cuisinePreference, string sweetTooth)
        {
            var user = await _dbContext.User.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.DietaryPreference = dietPreference;
            user.SpiceLevel = spiceLevel;
            user.CuisinePreference = cuisinePreference;
            user.SweetToothPreference = sweetTooth;

            _dbContext.User.Update(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }

}
