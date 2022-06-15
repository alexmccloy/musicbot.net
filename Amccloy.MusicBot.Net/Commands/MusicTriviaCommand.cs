using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Discord;
using Amccloy.MusicBot.Net.Trivia;
using Amccloy.MusicBot.Net.Trivia.MusicTrivia;
using Amccloy.MusicBot.Net.Utils;
using Amccloy.MusicBot.Net.Utils.RX;
using Discord.WebSocket;
using Lavalink4NET;

namespace Amccloy.MusicBot.Net.Commands;

public class MusicTriviaCommand : BaseDiscordCommand
{
    public override string CommandString => _commandString;
    private string _commandString = "musictrivia";
    protected override string SummaryHelpText => "Guess the songs before anyone else does";
    protected override string FullHelpText => _fullHelpText;
    private string _fullHelpText;

    // Dictionary of <playlist name, trivia provider that contains that playlist>
    private readonly Dictionary<string, IMusicTriviaQuestionProvider> _questionProviders = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly IScheduler _scheduler;
    
    private CancellationTokenSource _gameInProgress = null;

    public MusicTriviaCommand(ISchedulerFactory schedulerFactory,
                              IEnumerable<IMusicTriviaQuestionProvider> questionProviders)
        : base(schedulerFactory)
    {
        //TODO add a dependency to make sure we can ensure the bot is connected to the voice channel

        _scheduler = schedulerFactory.GenerateScheduler();

        // Populate list of playlists
        foreach (var questionProvider in questionProviders)
        {
            foreach (string playlist in questionProvider.Playlists)
            {
                _questionProviders.Add(playlist, questionProvider);
            }
        }
        
        // Populate help text
        var sb = new StringBuilder();
        sb.AppendLine("Play a game of music trivia.");
        sb.AppendLine($"Usage: !{_commandString} [playlist | source | random | stop] [questions]");
        sb.AppendLine($"Eg: !{_commandString} 90s_rock 10     Plays the playlist called 90s_rock with 10 questions");
        sb.AppendLine($"Eg: !{_commandString} spotify         Plays a random playlist from the spotify source");
        sb.AppendLine($"Eg: !{_commandString} random 25       Plays any random playlist with 25 questions");
        sb.AppendLine($"Eg: !{_commandString} stop            Stops an in progress game of music trivia");
        sb.AppendLine("Available playlists are:  -------------------------------------------------------------");

        foreach (var playlistName in _questionProviders.Keys)
        {
            sb.AppendLine($"[{_questionProviders[playlistName].Source}] {playlistName}");
        }

        _fullHelpText = sb.ToString();
    }

    /// <summary>
    /// Runs a game of music trivia
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    protected override async Task Execute(IDiscordInterface discordInterface, string[] args, SocketMessage commandMessage)
    {
        //TODO this should probably have a semaphore to make sure we dont get a race condition if lots of people spam to play
            
        // Check if we are already in a trivia game
        if (_gameInProgress != null)
        {
            // Check if they have requested to stop
            if (args.Length >= 2 && args[1].ToLower() == "stop")
            {
                await SendMessage("Stopping music trivia game due to someone requesting it be cancelled");
                _gameInProgress.Cancel();
            }
            else
            {
                // A game is already in progress so tell the user to leave
                await SendMessage($"A game is already in progress. Type '{_commandString} stop' to stop that game");
            }

            return;
        } 
        
        // Parse the arguments and extract a playlist to play
        string playlist;
        int gameLength;
        try
        {
            var potentialPlaylists = GetRequestedPlaylists(args);
            playlist = potentialPlaylists.PickRandom();

            gameLength = GetGameLength(args);
        }
        catch (ArgumentException e)
        {
            await SendMessage(e.Message);
            return;
        }
        
        // Start the game
        IDisposable subscription = null;
        try
        {
            _gameInProgress = new CancellationTokenSource();
            
            // Connect the bot to the voice channel
            var musicPlayer = await DiscordAudioManager.GetPlayer(commandMessage.Author.Id);

            var discordChatLog = new Subject<DiscordMessage>();
            subscription = discordInterface.MessageReceived.ObserveOn(_scheduler)
                                           .Where(message => message.Channel.Id == commandMessage.Channel.Id)
                                           .Subscribe(message =>
                                           {
                                               discordChatLog.OnNext(new DiscordMessage(message.Channel, message.Author.Username, message.Content));
                                           });

            var gameResult = new GameResults();
            
            //Start asking some questions
            var questions = await _questionProviders[playlist].GetQuestions(gameLength);
            for (int i = 0; i < questions.Count; i++)
            {
                if (_gameInProgress.IsCancellationRequested)
                {
                    break;
                }

                var question = questions[i];

                await SendMessage($"Question {i+1}:\n{question.Instruction}");
                var result = await question.ExecuteQuestion(discordChatLog.AsObservable(),
                                                            _questionProviders[playlist].QuestionDuration,
                                                            musicPlayer);

                if (result.Scores.Count == 0)
                {
                    // No one was correct
                    await SendMessage($"No one was correct! Answer:\n{question.Song.Name} by {question.Song.Artist}");
                }
                else
                {
                    gameResult.CombineWith(result);
                    //Someone was correct
                    await SendMessage($"{string.Join(", ", result.Scores.Where(score => score.Value > 0).Select(score => score.Key))} was correct!\n" +
                                      $"Answer: {question.Song.Name} by {question.Song.Artist}\n" +
                                      $"{gameResult.PrintScores()}");
                }
            }
            
            // Game is over, inform everyone of the winner
            await discordInterface.SendMessageAsync(commandMessage.Channel,
                                                    $"Game over, winner was {gameResult.GetWinner()}\n" +
                                                    $"{gameResult.PrintScores()}");

            await musicPlayer.DisconnectAsync();
        }
        finally
        {
            _gameInProgress = null;
            subscription?.Dispose();
        }

        Task SendMessage(string message) => discordInterface.SendMessageAsync(commandMessage.Channel, message);
    }

    #region Argument parsing

    /// <summary>
    /// Determine what type of playlist/s the user has requested
    /// </summary>
    private string[] GetRequestedPlaylists(string[] args)
    {
        // Make sure the user has actually requested something
        if (args.Length < 2) throw new ArgumentException("Command used incorrectly");
        
        // Check random mode
        if (args[1].ToLower() == "random")
        {
            // Return all the playlists that we know about
            return _questionProviders.Keys.ToArray();
        }
        
        // Check source mode
        var source = _questionProviders.Values .FirstOrDefault(
            provider => String.Equals(provider.Source, args[1], StringComparison.CurrentCultureIgnoreCase));
        
        if (source != null)
        {
            return source.Playlists;
        }
        
        // Find the specific playlist
        if (_questionProviders.ContainsKey(args[1]))
        {
            return new string[] { args[1] };
        }
        
        // We dont have the requested playlist
        throw new ArgumentException("Playlist or source not found");
    }

    private int GetGameLength(string[] args)
    {
        if (args.Length >= 3)
        {
            if (int.TryParse(args[2], out var result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException("Incorrect command usage, game length must be an integer");
            }
        }
        else
        {
            return 10;
        }
    }

    #endregion

}