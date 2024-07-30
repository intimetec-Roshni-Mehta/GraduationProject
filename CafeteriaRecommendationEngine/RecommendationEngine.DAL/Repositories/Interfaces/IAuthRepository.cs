using RecommendationEngine.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.DAL.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> Authenticate(string username);
        Task<int?> GetUserIdByUsername(string username); // New method signature
    }
}
