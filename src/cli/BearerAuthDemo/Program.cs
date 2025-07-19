using System;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace BearerAuthDemo
{
    public class Program
    {
        public static string ApiStatus { get; set; }

        public static void WriteLine(string value)
            => Console.WriteLine(value + Environment.NewLine);

        public static async Task<int> Main(string[] args)
        {
            var settings = LoadAppSettings();

            WriteLine("CLI User Agent: " + settings.UserAgent);

            using (var client = new ApiClient(settings.Secret, settings.BaseUrl, settings.UserAgent, settings.TimeoutSeconds))
            {
                if (!IsHealthy(client))
                {
                    WriteLine("Sorry, the API is currently offline.");
                    return await Task.FromResult(1);
                }

                WriteLine(ApiStatus);

                await ConfigureAccessTokenAsync(client, settings.Secret);

                var basicGradebookSearchCriteria = string.Empty;

                if (args.Length == 1)
                    basicGradebookSearchCriteria = args[0];

                var app = new Application(client, basicGradebookSearchCriteria);

                app.Run(WriteLine);
            }

            return await Task.FromResult(0);
        }

        private static async Task<Jwt> RetrieveJwt(ApiClient client, string secret)
        {
            var data = new { Secret = secret, Debug = true };

            var json = (await client.PostAsync("security/tokens/generate", data)).Data;

            var jwt = JsonSerializer.Deserialize<Jwt>(json);

            return jwt;
        }

        private static async Task ConfigureAccessTokenAsync(ApiClient client, string secret)
        {
            var jwt = await RetrieveJwt(client, secret);

            WriteLine($"{jwt.TokenType} access token generated. It expires in {jwt.ExpiresInMinutes} minutes.");

            client.UpdateClientSecret(jwt.AccessToken);
        }

        private static ApplicationSettings LoadAppSettings()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var settings = new ApplicationSettings();

            configuration.GetSection("BearerAuthDemo").Bind(settings);

            if (string.IsNullOrEmpty(settings.UserAgent))
                settings.UserAgent = UserAgentGenerator.Generate();

            if (settings.TimeoutSeconds < 1 || settings.TimeoutSeconds > 300)
                settings.TimeoutSeconds = 30;
                
            return settings;
        }

        private static bool IsHealthy(ApiClient client)
        {
            var response = client.Get("platform/health");

            if (response.Success)
            {
                ApiStatus = JsonSerializer.Deserialize<Health>(response.Data).Status;
            }

            return response.Success;
        }
    }
}