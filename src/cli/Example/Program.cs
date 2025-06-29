using System;
using System.Threading.Tasks;
using Example;
using Microsoft.Extensions.Configuration;

namespace ApiQuickstartExample
{
    public class Program
    {
        public static async Task<int> Main()
        {
            var settings = LoadAppSettings();
            
            using (var client = new ApiClient(settings.Secret, settings.BaseUrl, settings.UserAgent))
            {
                client.SetTimeout(TimeSpan.FromSeconds(30));

                if (IsOffline(client))
                {
                    Console.WriteLine("Sorry, the API is currently offline.");

                    return await Task.FromResult(1);
                }

                var status = client.Get("v1/status").Data;

                Console.WriteLine(status);

                var app = new Application(client);

                app.Run();
            }

            return await Task.FromResult(0);
        }

        private static ApplicationSettings LoadAppSettings()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var settings = new ApplicationSettings();

            configuration.GetSection("ApiQuickstart").Bind(settings);

            return settings;
        }

        private static bool IsOffline(ApiClient client)
        {
            var response = client.Get("v1/status");

            return !response.Success;
        }
    }
}