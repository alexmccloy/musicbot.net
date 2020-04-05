using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using NLog;

namespace Amccloy.MusicBot.Net.Commands
{
    public class CommandProcessingService : IHostedService
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IDiscordInterface _discordInterface;
        private IScheduler _scheduler;

        private IDisposable _subscription;

        private const string _commandPrefix = "!"; //TODO make this configurable

        private Dictionary<string, IDiscordCommand> _commandDict;

        public CommandProcessingService(IDiscordInterface discordInterface, ISchedulerFactory schedulerFactory)
        {
            _discordInterface = discordInterface;
            _scheduler = schedulerFactory.GenerateScheduler();
            
            _commandDict = new Dictionary<string, IDiscordCommand>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Starting Command Processing Service");
            RegisterCommands();
            _subscription = _discordInterface.MessageReceived
                                           .ObserveOn(_scheduler)
                                           .Where(message => message.Content.StartsWith(_commandPrefix))
                                           .Subscribe(async (message) => await HandleCommand(message));
            
            return Task.CompletedTask;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _subscription?.Dispose();
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Finds every class that implements IDiscordCommand interface and instantiates it, then puts it in a dictionary
        /// with the command text as the key
        /// </summary>
        private void RegisterCommands()
        {
            foreach (Type command in System.Reflection.Assembly
                                           .GetExecutingAssembly()
                                           .GetTypes()
                                           .Where(type => type.GetInterfaces().Contains(typeof(IDiscordCommand))))
            {
                try
                {
                    if (Activator.CreateInstance(command) is IDiscordCommand discordCommand)
                    {
                        _commandDict.Add(discordCommand.CommandString, discordCommand);
                    }
                    else
                    {
                        _logger.Error($"Failed to register command {command.Name}");
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"Failed to create command {command.Name}: {e.Message}");
                }
                
            }
        }
        
        private async Task HandleCommand(SocketMessage message)
        {
            _logger.Debug($"Received command: {message.Content}");

            var args = message.Content.Split(" ");
            var command = args[0].Substring(1);

            if (_commandDict.ContainsKey(command))
            {
                await _commandDict[command].Execute(_discordInterface, args, message);
            }
        }
    }
}