using RecomendationEngine.Services.Interfaces;
using RecommendationEngine.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReccomendationEngine.Controllers.Controllers
{
    public class AuthController
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        public async Task<string> Login(string username, string password)
        {
            try
            {
                var user = await authService.Authenticate(username, password);
                return $"Login successful; Role: {user.Role.RoleName}";
            }
            catch (UserNotFoundException)
            {
                return "Login failed: User not found";
            }
            catch (IncorrectPasswordException)
            {
                return "Login failed: Incorrect password";
            }
            catch (Exception ex)
            {
                return $"Login failed: {ex.Message}";
            }
        }

        public string Logout(string username)
        {
         
            return null;
        }
    }
}
