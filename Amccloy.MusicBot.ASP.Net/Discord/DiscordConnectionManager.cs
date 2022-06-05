using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Amccloy.MusicBot.Asp.Net.Commands;
using Amccloy.MusicBot.Asp.Net.Configuration;
using Amccloy.MusicBot.Asp.Net.Utils.RX;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NLog;

namespace Amccloy.MusicBot.Asp.Net.Discord
{
    public class DiscordConnectionManager : BackgroundService
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        
        //TODO eventually turn these into a list
        private DiscordSocketClient _discordSocketClient;
        private IDiscordInterface _discordInterface; 
        CommandProcessingService _commandProcessingService;
                
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly DiscordOptions _discordOptions;

        public DiscordConnectionManager(ISchedulerFactory schedulerFactory, IOptions<DiscordOptions> discordOptions)
        {
            _schedulerFactory = schedulerFactory;
            _discordOptions = discordOptions.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
                _logger.Info($"Setting up discord client");

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

                _discordInterface = new DiscordInterface(_discordSocketClient);
                _commandProcessingService = new CommandProcessingService(_discordInterface, _schedulerFactory); //TODO why is this not created by DI?
                await _commandProcessingService.StartAsync(CancellationToken.None);
                //TODO when monitoring users in channels create that service here

        }
    }


}