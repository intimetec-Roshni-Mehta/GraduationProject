using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.DAL.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<bool> UpdateProfile(int? user, string dietPreference, string spiceLevel, string cuisinePreference, string sweetTooth);
    }
}
