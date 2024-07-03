using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.Common.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string username)
            : base($"User with username '{username}' was not found.")
        {
        }
    }

    public class IncorrectPasswordException : Exception
    {
        public IncorrectPasswordException()
            : base("The password is incorrect.")
        {
        }
    }
}
