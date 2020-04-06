using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Commands;
using Amccloy.MusicBot.Net.Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
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
            

            host.AddNLog()
                .ApplyConfig<Program>()
                .ConfigureServices((context, services) => services.AddSingleton<DiscordSocketClient>())
                .ConfigureServices((context, services) => services.AddSingleton<IDiscordInterface, DiscordInterface>())
                .ConfigureServices((context, services) => services.AddSingleton<ISchedulerFactory, DefaultSchedulerFactory>())
                
                .ConfigureServices((context, services) => services.AddHostedService<CommandProcessingService>())
                .ConfigureServices((context, services) => services.AddHostedService<InitService>())
                ;

            Console.WriteLine("Starting application");
            await host.RunConsoleAsync().ConfigureAwait(false);
        }
    }
    
    public class InitService : IHostedService
    {
        private readonly IDiscordInterface _discordInterface;

        public InitService(IDiscordInterface discordInterface)
        {
            _discordInterface = discordInterface;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _discordInterface.Init();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _discordInterface.Stop();
        }
    }
}