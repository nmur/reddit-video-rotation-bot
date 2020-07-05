using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedditVideoRotationBot.Configurations;
using RedditVideoRotationBot.Interfaces;
using Refit;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace RedditVideoRotationBot
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
#pragma warning disable RCS1018 // Add accessibility modifiers.
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable IDE0060 // Remove unused parameter
        static void Main(string[] args)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore RCS1163 // Unused parameter.
#pragma warning restore RCS1018 // Add accessibility modifiers.
        {
            Console.WriteLine($"Starting...");
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();

            var redditHelper = new RedditHelper(
                serviceProvider.GetService<IRedditClientWrapper>(),
                serviceProvider.GetService<IRedditMessageHandler>());
            redditHelper.MonitorUnreadMessages();

            System.Threading.Thread.Sleep(1000 * 60 * 60 * 72);
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            var config = LoadConfiguration();
            services.AddSingleton(config);
            var redditClientConfiguration = GetRedditClientSegmentFromConfiguration(config);
            var gfyCatApiConfiguration = GetGfyCatApiSegmentFromConfiguration(config);

            Console.WriteLine($"redditClientConfiguration.GetAppId: {redditClientConfiguration.GetAppId()}");

            services.AddRefitClient<IGfyCatApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.gfycat.com/v1"));
            services.AddRefitClient<IGfyCatFileDropApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri("https://filedrop.gfycat.com"));

            services.AddSingleton<IRedditClientConfiguration>(redditClientConfiguration);
            services.AddSingleton<IGfyCatApiConfiguration>(gfyCatApiConfiguration);
            services.AddSingleton<IVideoUploader, GfyCatVideoUploader>();
            services.AddSingleton<IVideoDownloader, VideoDownloader>();
            services.AddSingleton<IAudioDownloader, AudioDownloader>();
            services.AddSingleton<IMediaProcessor, MediaProcessor>();
            services.AddSingleton<IVideoRotator, FfmpegVideoRotator>();
            services.AddSingleton<IRedditMessageHandler, RedditMessageHandler>();
            services.AddSingleton<IRedditClientWrapper, RedditClientWrapper>();

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

        private static RedditClientConfiguration GetRedditClientSegmentFromConfiguration(IConfiguration config)
        {
            return new RedditClientConfiguration(
                config["RedditClient:AppId"],
                config["RedditClient:AppSecret"],
                config["RedditClient:RefreshToken"]);
        }

        private static GfyCatApiConfiguration GetGfyCatApiSegmentFromConfiguration(IConfiguration config)
        {
            return new GfyCatApiConfiguration(
                config["GfyCatApi:ClientId"],
                config["GfyCatApi:ClientSecret"],
                int.Parse(config["GfyCatApi:UploadTimeoutInMs"]));
        }
    }
}
