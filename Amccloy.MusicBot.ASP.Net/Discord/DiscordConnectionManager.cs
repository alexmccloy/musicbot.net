using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net;
using Amccloy.MusicBot.Net.Commands;
using Amccloy.MusicBot.Net.Discord;
using DataAccessLibrary;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using NLog;

namespace Amccloy.MusicBot.Asp.Net.Discord
{
    public class DiscordConnectionManager : BackgroundService
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        
        //TODO eventually turn these into a list
        private readonly DiscordSocketClient _discordSocketClient;
        private IDiscordInterface _discordInterface; 
        CommandProcessingService _commandProcessingService;
                
        private readonly IDiscordApiTokenData _apiTokenData;
        private readonly ISchedulerFactory _schedulerFactory;

        public DiscordConnectionManager(DiscordSocketClient discordSocketClient, IDiscordApiTokenData apiTokenData, ISchedulerFactory schedulerFactory)
        {
            _discordSocketClient = discordSocketClient;
            _apiTokenData = apiTokenData;
            _schedulerFactory = schedulerFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var token in await _apiTokenData.GetAllKeys())
            {
                _logger.Info($"Setting up discord client for Server {token.ServerName}");


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
                    _logger.Info($"Discord client for {token.ServerName} is ready");
                    return Task.CompletedTask;
                };
                //TODO handle disconnected event and try to reconnect

                await _discordSocketClient.LoginAsync(TokenType.Bot, token.ApiKey);
                await _discordSocketClient.StartAsync();

                _discordInterface = new DiscordInterface(_discordSocketClient);
                _commandProcessingService = new CommandProcessingService(_discordInterface, _schedulerFactory);
                await _commandProcessingService.StartAsync(CancellationToken.None);
                //TODO when monitoring users in channels create that service here

                break; //TODO only use 1 server at a time for now
            }
        }
    }


}