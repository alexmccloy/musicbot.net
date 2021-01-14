using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amccloy.MusicBot.Asp.Net.Diagnostics;
using Amccloy.MusicBot.Asp.Net.Discord;
using Amccloy.MusicBot.Asp.Net.Utils.RX;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using NLog;

namespace Amccloy.MusicBot.Asp.Net.Commands
{
    /// <summary>
    /// Hosted service responsible for processing commands from users. It determines which class should handle the
    /// command and then passes the relevant details down.
    /// </summary>
    public class CommandProcessingService : BackgroundService
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IDiscordInterface _discordInterface;
        private readonly ISchedulerFactory _schedulerFactory; //Required to pass down to dependent classes
        private readonly IActivityMonitor _activityMonitor;
        private readonly IScheduler _scheduler;

        private IDisposable _subscription;

        private const string _commandPrefix = "!"; //TODO make this configurable

        private readonly Dictionary<string, BaseDiscordCommand> _commandDict;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="discordInterface">Interface to send and receive discord messages</param>
        /// <param name="schedulerFactory">Generator for Reactive Schedulers</param>
        /// <param name="activityMonitor">Allows the commands to log an event occuring</param>
        public CommandProcessingService(IDiscordInterface discordInterface, ISchedulerFactory schedulerFactory, IActivityMonitor activityMonitor)
        {
            _discordInterface = discordInterface;
            _schedulerFactory = schedulerFactory;
            _activityMonitor = activityMonitor;
            _scheduler = schedulerFactory.GenerateScheduler();
            
            _commandDict = new Dictionary<string, BaseDiscordCommand>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Info("Starting Command Processing Service");
            await RegisterCommands();
            
            _logger.Info("Subscribing to incoming messages");
            _discordInterface.MessageReceived
                             .ObserveOn(_scheduler)
                             .Where(message => message.Content.StartsWith(_commandPrefix))
                             .Subscribe(async message => await HandleCommand(message), stoppingToken);

            if (_logger.IsTraceEnabled)
            {
                _discordInterface.MessageReceived.ObserveOn(_scheduler)
                                 .Subscribe(async message =>
                                 {
                                     _logger.Trace($"Got message {message.Content}");
                                 }, stoppingToken);
            } 
        }


        /// <summary>
        /// Finds every class that implements IDiscordCommand interface and instantiates it, then puts it in a dictionary
        /// with the command text as the key
        /// </summary>
        private async Task RegisterCommands()
        {
            foreach (Type command in System.Reflection.Assembly
                                           .GetExecutingAssembly()
                                           .GetTypes()
                                           .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(BaseDiscordCommand))))
            {
                try
                {
                    if (Activator.CreateInstance(command, _schedulerFactory, _activityMonitor) is BaseDiscordCommand discordCommand)
                    {
                        await discordCommand.Init();
                        _commandDict.Add(discordCommand.CommandString, discordCommand);
                        _logger.Info($"Added command: {discordCommand.CommandString}");
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
        
        /// <summary>
        /// Determines what command the user requested and executes that command.
        /// If the command is unknown then prints help text instead
        /// </summary>
        /// <param name="message"></param>
        private async Task HandleCommand(SocketMessage message)
        {
            _logger.Debug($"Received command: {message.Content}");

            var args = message.Content.Split(" ");
            var command = args[0].Substring(1);

            if (_commandDict.ContainsKey(command))
            {
                await _commandDict[command].ExecuteCommand(_discordInterface, args, message);
            }
            else if (command == "help")
            {
                // Expected help command:
                // !help -> prints summary help text for each command
                // !help command -> prints the full help text for that command
                if (args.Length >= 2 && _commandDict.ContainsKey(args[1]))
                {
                    await _discordInterface.SendMessageAsync(message.Channel, _commandDict[args[1]].PrintFullHelpText());
                }
                else
                {
                    await _discordInterface.SendMessageAsync(message.Channel, String.Join('\n', _commandDict.Values.Select(x => x.PrintSummaryHelpText())));
                }
            }
        }
    }
}