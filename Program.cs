using System;
using System.Threading.Tasks;

namespace IncMediaBackgroundService
{
    class Program
    {
        private static JsonRpcServer Server { get; set; } = new JsonRpcServer();

        static async Task Main(string[] args)
        {
            await Server.StartAsync();
        }
    }
}
