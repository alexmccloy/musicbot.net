using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Discord;
using Amccloy.MusicBot.Net.Trivia;
using Discord.WebSocket;
using NLog;

namespace Amccloy.MusicBot.Net.Commands
{
    public class TriviaCommand : BaseDiscordCommand
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private List<ITriviaQuestionProvider> _questionProviders;
        private string _fullHelpText;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public TriviaCommand(ISchedulerFactory schedulerFactory)
        {
            //TODO work out how to pass this in
            _schedulerFactory = schedulerFactory;
        }

        public override string CommandString => "trivia";
        protected override string SummaryHelpText => "Plays an interactive game of trivia";

        protected override string FullHelpText => _fullHelpText;

        public override async Task Execute(IDiscordInterface discordInterface, string[] args, SocketMessage rawMessage)
        {
            if (args.Length < 2)
            {
                // Command was used wrong, print usage info and exit
                await discordInterface.SendMessageAsync(rawMessage.Channel, FullHelpText);
            }
        }

        public override async Task Init()
        {
            //Find all the ITriviaQuestionProviders and populate a list so that it can be used in help text and to 
            //    select which source to use when playing
            _questionProviders = new List<ITriviaQuestionProvider>();

            foreach (var triviaProvider in System.Reflection.Assembly
                                                 .GetExecutingAssembly()
                                                 .GetTypes()
                                                 .Where(type => type.IsClass && ! type.IsAbstract && type.GetInterfaces().Contains(typeof(ITriviaQuestionProvider))))
            {
                try
                {
                    if (Activator.CreateInstance(triviaProvider) is ITriviaQuestionProvider questionProvider)
                    {
                        await questionProvider.Init();
                        _questionProviders.Add(questionProvider);
                    }
                    else
                    {
                        _logger.Error($"Failed to initialise trivia question provider {triviaProvider.Name}");
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"Failed to create trivia question provider {triviaProvider.Name}: {e.Message}");
                }
            }
            
            // Populate the help text so that players can see the list of available trivia providers
            var sb = new StringBuilder();
            sb.AppendLine("Game of trivia. Usage: !trivia source <question count>");
            sb.AppendLine("Available sources are:");
            foreach (var triviaQuestionProvider in _questionProviders)
            {
                sb.AppendLine($"    {triviaQuestionProvider.Name} - {triviaQuestionProvider.Description}");
            }

            _fullHelpText = sb.ToString();
        }
    }
}