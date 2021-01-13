using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Amccloy.MusicBot.Asp.Net.Discord;
using Amccloy.MusicBot.Net.Discord;
using Amccloy.MusicBot.Net.Trivia;
using Discord.WebSocket;
using NLog;

namespace Amccloy.MusicBot.Net.Commands
{
    /// <summary>
    /// Command for playing an interactive game of trivia
    /// </summary>
    public class TriviaCommand : BaseDiscordCommand
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private Dictionary<string, ITriviaQuestionProvider> _questionProviders;
        private string _fullHelpText;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private IScheduler _scheduler;


        public override string CommandString => "trivia";
        protected override string SummaryHelpText => "Plays an interactive game of trivia";

        protected override string FullHelpText => _fullHelpText;

        
        public TriviaCommand(ISchedulerFactory schedulerFactory)
            : base(schedulerFactory)
        {
            _scheduler = schedulerFactory.GenerateScheduler();
        }
        
        /// <summary>
        /// Runs a game of trivia with the provided trivia source.
        /// Expected args:
        ///    1. Trivia source name that matches a provider in _questionProviders
        ///    2. (Optional) The number of rounds to play, default is 10
        /// </summary>
        /// <param name="discordInterface"></param>
        /// <param name="args"></param>
        /// <param name="rawMessage"></param>
        public override async Task Execute(IDiscordInterface discordInterface, string[] args, SocketMessage rawMessage)
        {
            if (!TryExtractArgs(args, out var questionProvider, out var gameLength))
            {
                // Command was used wrong, print usage info and exit
                await discordInterface.SendMessageAsync(rawMessage.Channel, FullHelpText);
                return;
            }
            
            Subject<DiscordMessage> subject = new Subject<DiscordMessage>();
            discordInterface.MessageReceived.ObserveOn(_scheduler)
                            .Where(message => message.Channel.Id == rawMessage.Channel.Id)
                            .Subscribe(message =>
                            {
                                subject.OnNext(new DiscordMessage(message.Channel, message.Author.Username, message.Content));
                            });
            
            var gameResult = new GameResults();

            // Start asking questions
            var questions = await questionProvider.GetQuestions(gameLength); //TODO this might need to be async
            for (int i =0; i< questions.Count; i++)
            {
                await discordInterface.SendMessageAsync(rawMessage.Channel, $"Question {i + 1}:\n{questions[i].Question}");
                var result = await questions[i].ExecuteQuestion(subject.AsObservable(), questionProvider.QuestionDuration);

                if (result.Scores.Count == 0)
                {
                    // No one was correct
                    await discordInterface.SendMessageAsync(rawMessage.Channel, $"No one was correct! Answer:\n{questions[i].Answer}");
                }
                else
                {
                    gameResult.CombineWith(result);
                    //Someone was correct
                    await discordInterface.SendMessageAsync(rawMessage.Channel, $"{String.Join(", ", result.Scores.Where(score => score.Value > 0).Select(score => score.Key))} was correct!\n" +
                                                                                $"{gameResult.PrintScores()}");
                }
            }
            
            // Game is over, inform everyone of the winner
            await discordInterface.SendMessageAsync(rawMessage.Channel,
                                                    $"Game over, winner was {gameResult.GetWinner()}\n" +
                                                    $"{gameResult.PrintScores()}");
        }

        /// <summary>
        /// Extracts and parses the args. Returns false if the args are incorrect
        /// </summary>
        private bool TryExtractArgs(string[] args, 
                                    out ITriviaQuestionProvider questionProvider, 
                                    out int gameLength)
        {
            questionProvider = null;
            gameLength = 10;
            
            if (args.Length < 2)
            {
                return false; // Not enough arguments specified
            }

            if (!_questionProviders.TryGetValue(args[1].ToLower(), out questionProvider))
            {
                return false; // The game that was asked for does not exist
            }

            if ((args.Length >= 3 && !int.TryParse(args[2], out gameLength)) || gameLength < 1)
            {
                return false; // The number of rounds asked for was not a positive integer
            }

            return true;
        }

        /// <summary>
        /// Finds all available trivia providers and uses them to populate the _questionProviders list.
        /// Then updates the help text with this new list
        /// </summary>
        public override async Task Init()
        {
            //Find all the ITriviaQuestionProviders and populate a list so that it can be used in help text and to 
            //    select which source to use when playing
            _questionProviders = new Dictionary<string, ITriviaQuestionProvider>();

            foreach (var triviaProvider in System.Reflection.Assembly
                                                 .GetExecutingAssembly()
                                                 .GetTypes()
                                                 .Where(type => type.IsClass && ! type.IsAbstract && type.GetInterfaces().Contains(typeof(ITriviaQuestionProvider))))
            {
                try
                {
                    if (Activator.CreateInstance(triviaProvider, SchedulerFactory) is ITriviaQuestionProvider questionProvider)
                    {
                        await questionProvider.Init();
                        _questionProviders.Add(questionProvider.Name.ToLower(), questionProvider);
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
            foreach (var triviaQuestionProvider in _questionProviders.Values)
            {
                sb.AppendLine($"    {triviaQuestionProvider.Name} - {triviaQuestionProvider.Description}");
            }

            _fullHelpText = sb.ToString();
        }

    }
}