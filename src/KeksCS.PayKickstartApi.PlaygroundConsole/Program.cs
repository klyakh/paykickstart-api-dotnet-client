using Microsoft.Extensions.Configuration;
using System;

namespace KeksCS.PayKickstartApi.PlaygroundConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.Development.json", true, true)
                .Build();



            var authToken = config["AuthToken"];
            if(!string.IsNullOrEmpty(authToken))
            {
                Console.WriteLine("AuthToken not configured");
                return;
            }

            using (var apiClient = new PayKickstartApiClient(authToken))
            {

            }
        }
    }
}
