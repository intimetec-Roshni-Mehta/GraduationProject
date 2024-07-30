using RecommendationEngine.DAL.DbConnection;
using RecommendationEngine.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomendationEngine.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User> Authenticate(string username, string password);
        Task<int?> GetUserIdByUsername(string username); // New method signature
    }

}
