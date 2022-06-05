using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog.Web;

namespace Amccloy.MusicBot.Net
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, configBuilder) =>
                {
                    configBuilder.AddEnvironmentVariables("MUSICBOTNET_");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseNLog(new NLogAspNetCoreOptions()
                {
                    ReplaceLoggerFactory = true,
                    
                });
    }
}
