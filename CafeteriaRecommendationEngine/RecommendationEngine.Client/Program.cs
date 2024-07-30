using RecommendationEngine.Communication.SocketClient;
using System;
using System.Threading.Tasks;

namespace RecommendationEngine.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketMessenger.StartClient();
        }
    }

}
