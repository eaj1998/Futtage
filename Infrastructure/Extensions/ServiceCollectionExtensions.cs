
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IO;
using Futtage.Core.Services;
using Futtage.Infrastructure.Logging;
using Futtage.Infrastructure.Configuration;
using Futtage.Presentation.Presenters;

namespace Futtage.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFuttageServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<AppSettings>(configuration);

            var appSettings = new AppSettings();
            configuration.Bind(appSettings);

            services.AddSingleton<ILogger>(provider =>
                new FileLogger(Path.Combine(appSettings.LoggingPath, "futtage.log")));

            services.AddSingleton<IVideoProcessingService>(provider =>
                new VideoProcessingService(
                    provider.GetRequiredService<ILogger>(),
                    appSettings.FFmpegPath
                ));

            services.AddSingleton<IYouTubeService>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger>();

                var clientId = Environment.GetEnvironmentVariable("YOUTUBE_CLIENT_ID") ?? appSettings.YouTube.ClientId;
                var clientSecret = Environment.GetEnvironmentVariable("YOUTUBE_CLIENT_SECRET") ?? appSettings.YouTube.ClientSecret;

                System.Diagnostics.Debug.WriteLine($"YouTube Client ID configurado: {!string.IsNullOrEmpty(clientId)}");
                System.Diagnostics.Debug.WriteLine($"YouTube Client Secret configurado: {!string.IsNullOrEmpty(clientSecret)}");

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    logger.LogWarning("Credenciais do YouTube não configuradas - usando modo simulado");
                }

                return new Core.Services.YouTubeService(
                    logger,
                    clientId,
                    clientSecret,
                    appSettings.YouTube.ApplicationName
                );
            });

            services.AddScoped<IFileService, FileService>();

            services.AddTransient<MainPresenter>();

            return services;
        }
    }
}