using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Amccloy.MusicBot.Net
{
    public static class HostExtensions
    {
        public static IHostBuilder AddNLog(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((Action<IServiceCollection>) (services => services.AddLogging((Action<ILoggingBuilder>) (config =>
            {
                config.SetMinimumLevel(LogLevel.Trace);
                config.AddNLog(new NLogProviderOptions()
                {
                    CaptureMessageProperties = true,
                    CaptureMessageTemplates = true,
                    IncludeScopes = true,
                    
                });
            }))));
        }

        public static IHostBuilder ApplyConfig<T>(this IHostBuilder builder) where T : class
        {
            builder.ConfigureHostConfiguration(config =>
            {
                config.AddJsonFile("config.json", true)
                      .AddEnvironmentVariables()
                      .AddUserSecrets<T>();
            });
            return builder;
        }
    }
}