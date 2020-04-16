using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace RedditVideoRotationBot
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Starting...");
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetService<IRedditClientWrapper>();
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            var config = LoadConfiguration();
            services.AddSingleton(config);

            var redditClientConfiguration = new RedditClientConfiguration(
                config["RedditClient:AppId"],
                config["RedditClient:AppSecret"],
                config["RedditClient:RefreshToken"]);

            Console.WriteLine($"redditClientConfiguration.GetAppId: {redditClientConfiguration.GetAppId()}");

            services.AddSingleton<IRedditClientConfiguration>(redditClientConfiguration);
            services.AddTransient<IRedditClientWrapper, RedditClientWrapper>();

            return services;
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
