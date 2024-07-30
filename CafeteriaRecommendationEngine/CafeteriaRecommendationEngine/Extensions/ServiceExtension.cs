using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecomendationEngine.Services.Implementation;
using RecomendationEngine.Services.Interfaces;
using RecommendationEngine.DAL.DbConnection;
using RecommendationEngine.DAL.Repositories.Implementation;
using RecommendationEngine.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.Server.Extensions
{
    public class ServiceExtension
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RecommendationEngineContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("RecommendationEngine.Server")));

            #region Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IChefService, ChefService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            #endregion

            #region Repositories
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IVotedItemRepository, VotedItemRepository>();
            services.AddScoped<IRecommendationRepository, RecommendationRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            #endregion

        }
    }
}
