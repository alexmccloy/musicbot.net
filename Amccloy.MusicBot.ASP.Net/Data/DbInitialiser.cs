using System.Threading;
using System.Threading.Tasks;
using DataAccessLibrary;
using Microsoft.Extensions.Hosting;

namespace Amccloy.MusicBot.Asp.Net.Data
{
    public class DbInitialiser : BackgroundService
    {
        private readonly IDiscordApiTokenData _discordApiTokenData;

        public DbInitialiser(IDiscordApiTokenData discordApiTokenData)
        {
            _discordApiTokenData = discordApiTokenData;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _discordApiTokenData.Init();
        }
    }
}