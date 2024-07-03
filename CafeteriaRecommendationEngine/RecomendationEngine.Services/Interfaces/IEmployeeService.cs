using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomendationEngine.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<string> VoteForItem(int userId, int itemId);
        Task<string> GiveFeedback(int userId, int itemId, int rating, string comment);
    }
}
