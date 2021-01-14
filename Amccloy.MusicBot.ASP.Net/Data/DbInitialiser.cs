using System.Threading;
using System.Threading.Tasks;
using DataAccessLibrary;
using DataAccessLibrary.ActivityLogging;
using DataAccessLibrary.DiscordApiToken;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Hosting;

namespace Amccloy.MusicBot.Asp.Net.Data
{
    public class DbInitialiser : BackgroundService
    {
        private readonly IDiscordApiTokenData _discordApiTokenData;
        private readonly IActivityData _activityData;

        public DbInitialiser(IDiscordApiTokenData discordApiTokenData,
                             IActivityData activityData)
        {
            _discordApiTokenData = discordApiTokenData;
            _activityData = activityData;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _discordApiTokenData.Init();
            await _activityData.Init();
        }
    }
}