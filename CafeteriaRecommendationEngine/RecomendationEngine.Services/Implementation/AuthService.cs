using Microsoft.EntityFrameworkCore;
using RecommendationEngine.DAL.DbConnection;
using RecommendationEngine.DataModel.Models;
using RecomendationEngine.Services.Interfaces;
using System.Threading.Tasks;
using RecommendationEngine.DAL.Repositories.Interfaces;
using RecommendationEngine.DAL.Repositories.Implementation;
using RecommendationEngine.Common.Exceptions;

namespace RecomendationEngine.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            try
            {
                var user = await _authRepository.Authenticate(username);

                if (user == null)
                {
                    throw new UserNotFoundException(username);
                }

                if (!string.Equals(user.Password, password))
                {
                    throw new IncorrectPasswordException();
                }

                return user;
            }
            catch (UserNotFoundException)
            {
                throw;
            }
            catch (IncorrectPasswordException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred during authentication.", ex);
            }
        }

        public async Task<int?> GetUserIdByUsername(string username)
        {
            var user = await _authRepository.Authenticate(username);
            return user?.UserId;
        }
    }

}
