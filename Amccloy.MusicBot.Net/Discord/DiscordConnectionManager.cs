using System.Threading;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Commands;
using Amccloy.MusicBot.Net.Configuration;
using Amccloy.MusicBot.Net.Utils.RX;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NLog;

namespace Amccloy.MusicBot.Net.Discord
{
    public class DiscordConnectionManager
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        //TODO eventually turn these into a list
        private DiscordSocketClient _discordSocketClient;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly DiscordOptions _discordOptions;
        private IDiscordInterface _discordInterface = null;

        public DiscordConnectionManager(ISchedulerFactory schedulerFactory,
                                        IOptions<DiscordOptions> discordOptions)
        {
            _schedulerFactory = schedulerFactory;
            _discordOptions = discordOptions.Value;
        }

        public async Task<IDiscordInterface> GetDiscordClient()
        {
            // For now just always return the same client to everyone
            if (_discordInterface == null)
            {
                _logger.Info($"Setting up discord client");
                _logger.Info($"SEcret token is {_discordOptions.BotToken}");

                _discordSocketClient = new DiscordSocketClient(new DiscordSocketConfig
                {
                    MessageCacheSize = 0,
                    AlwaysDownloadUsers = true,


                    GatewayIntents =
                        GatewayIntents.Guilds |
                        GatewayIntents.GuildMembers |
                        GatewayIntents.GuildMessageReactions |
                        GatewayIntents.GuildMessages |
                        GatewayIntents.GuildVoiceStates
                });

                _discordSocketClient.Log += message =>
                {
                    if (message.Exception != null)
                    {
                        _logger.Warn($"DiscordClient: {message.Message} - {message.Exception.Message}");
                    }
                    else
                    {
                        _logger.Info($"DiscordClient: {message.Message}");
                    }

                    return Task.CompletedTask;
                };
                _discordSocketClient.Ready += () =>
                {
                    _logger.Info($"Discord client is ready");
                    return Task.CompletedTask;
                };

                _discordSocketClient.Disconnected += exception =>
                {
                    _logger.Error($"Discord Client disconnected: {exception.Message}");
                    return Task.CompletedTask;
                };
                //TODO handle disconnected event and try to reconnect


                await _discordSocketClient.LoginAsync(TokenType.Bot, _discordOptions.BotToken);
                await _discordSocketClient.StartAsync();

                //TODO when monitoring users in channels create that service here

                _discordInterface = new DiscordInterface(_discordSocketClient);
            }

            return _discordInterface;
        }
    }

}