using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amccloy.MusicBot.Net
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            var host = new HostBuilder();

            host.AddNLog2()
                .ConfigureServices((context, services) => services.AddSingleton<DiscordSocketClient>())
                
                .ConfigureServices((context, services) => services.AddHostedService<DiscordService>())
                ;

            Console.WriteLine("Starting application");
            await host.RunConsoleAsync().ConfigureAwait(false);
        }
    }
}