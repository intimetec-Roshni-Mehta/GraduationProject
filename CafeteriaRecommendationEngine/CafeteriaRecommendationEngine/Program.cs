using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RecommendationEngine.Communication.SocketServer;
using RecommendationEngine.Server.Extensions;
using System.Threading.Tasks;


namespace RecommendationEngine
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    ServiceExtension.Configure(services, context.Configuration);
                })
                .Build();

            var services = host.Services;
            await SocketListener.StartServer(services);
        }
    }
}
