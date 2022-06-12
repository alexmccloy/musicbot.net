using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Commands;
using Amccloy.MusicBot.Net.Dbo;
using Amccloy.MusicBot.Net.Discord;
using Amccloy.MusicBot.Net.Utils.RX;
using Lavalink4NET;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;

namespace Amccloy.MusicBot.Net.Trivia.MusicTrivia;

public class MusicTriviaQuestion : IMusicTriviaQuestion, IDisposable
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IAudioService _audioService;

    public MusicTriviaQuestionType QuestionType => MusicTriviaQuestionType.Standard;
    public string Instruction => "Guess the Artist and the Song Name";
    public Song Song { get; }

    private IDisposable _subscription = null;

    private static Regex _regex = new Regex(@"[^a-zA-Z0-9 ]");

    public MusicTriviaQuestion(Song song, ISchedulerFactory schedulerFactory, IAudioService audioService)
    {
        _schedulerFactory = schedulerFactory;
        _audioService = audioService;
        Song = song;
    }

    [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
    public async Task<GameResults> ExecuteQuestion(IObservable<DiscordMessage> chat, TimeSpan maxDuration, LavalinkPlayer musicPlayer)
    {
        var result = new GameResults();
        var tcs = new TaskCompletionSource<bool>(); // True if someone was right, false if we timed out
        var cancellationTokenSource = new CancellationTokenSource(maxDuration);
        cancellationTokenSource.Token.Register(() => tcs.TrySetResult(false));
        

        bool phase1 = true; // When this is true the round ends when the first person gets the question right,
        // when it is false we are in the 1 second grace period
                            
        // Subscribe to incoming answers from users
        _subscription = chat.ObserveOn(_schedulerFactory.GenerateScheduler())
                            .Subscribe(message =>
                            {
                                int score = CalculateScore(message.MessageText);
                                if (score > 0)
                                {
                                    // Someone was correct,
                                    result.AddScore(message.UserName, score);

                                    if (phase1)
                                    {
                                        //Continue to phase 2
                                        cancellationTokenSource.Cancel();
                                        tcs.TrySetResult(true);
                                    }
                                }
                            });
        
        // Start playing the song
        var track = await _audioService.GetTrackAsync($"{Song.Artist} {Song.Name}", SearchMode.YouTube)
                 ?? throw new DiscordCommandException($"Cannot find track {Song.Artist}-{Song.Name} on youtube");
        await musicPlayer.PlayAsync(track, endTime:maxDuration);
        
        // Phase 1 - either we time out or someone get the answer right
        var answerFound = await tcs.Task;

        if (answerFound)
        {
            // Phase 2 - 1 second grace period to account for lag when multiple people get the answer right
            phase1 = false;
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        
        // Stop playing the song
        // await musicPlayer.StopAsync(disconnect:false);

        return result;
    }

    /// <summary>
    /// Checks if the answer was correct and returns how many points they got.
    /// Artist correct = 1 point
    /// Song correct = 1 point
    /// Both correct = 3 points
    /// </summary>
    private int CalculateScore(string rawGuess)
    {
        bool artistCorrect = false;
        bool songCorrect = false;
        
        var guess = RemoveFormatting(rawGuess);
        var song = RemoveFormatting(Song.Name);
        var artist = RemoveFormatting(Song.Artist);

        artistCorrect = guess.Contains(artist);
        songCorrect = guess.Contains(song);

        //TODO surely theres a neater way to do this
        return (artistCorrect ? 1 : 0)
             + (songCorrect ? 1 : 0)
             + (songCorrect && artistCorrect ? 1 : 0);
    }

    /// <summary>
    /// Removes the following from a string:
    ///  - Non-alphanumeric characters (except for spaces)
    ///  - Whitespace from the edges of the string
    /// </summary>
    private string RemoveFormatting(string input)
    {
        string trimmed = input.ToLower().Trim();
        return _regex.Replace(trimmed, "");
    }
    

    public void Dispose()
    {
        _subscription?.Dispose();
    }
}