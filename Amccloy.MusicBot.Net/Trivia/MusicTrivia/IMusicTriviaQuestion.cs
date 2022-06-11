using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Commands;
using Amccloy.MusicBot.Net.Discord;
using Amccloy.MusicBot.Net.Utils.RX;
using Lavalink4NET;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;

namespace Amccloy.MusicBot.Net.Trivia.MusicTrivia;

/// <summary>
/// A single Music trivia question, where a song will play and everyone has to guess it
/// </summary>
public interface IMusicTriviaQuestion
{
    MusicTriviaQuestionType QuestionType { get; }
    
    Song Song { get; }

    /// <summary>
    /// This method should run the entire question. It will decide whether the question should be time based or end
    /// when the first person gets it correct. It can interact with the user (eg. to provide hints).
    /// When the question has been answered it will return a list of users and how many points the got
    /// Points return can be negative if the user should loose points
    /// </summary>
    /// <param name="chat">A feed of chat messages that are considered answers to the question</param>
    /// <param name="maxDuration">The max duration the question should run for. If the test runs for longer than
    /// this the calling method may time it out and it will not be able to return a value</param>
    /// <param name="musicPlayer">The music player to control what track is played. It is assumed this is already
    /// connected to the correct voice channel</param>
    /// <returns>A list of users and the points they earned or lost while answering this question</returns>
    Task<GameResults> ExecuteQuestion(IObservable<DiscordMessage> chat, TimeSpan maxDuration, LavalinkPlayer musicPlayer);
}

public class MusicTriviaQuestion : IMusicTriviaQuestion, IDisposable
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IAudioService _audioService;
    public MusicTriviaQuestionType QuestionType { get; }
    public Song Song { get; }

    private IDisposable _subscription = null;

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
            if (IsAnswerCorrect(message.MessageText) > 1)
            {
                // Someone was correct,
                result.AddScore(message.UserName, 1);

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
        await musicPlayer.PlayAsync(track);
        
        // Phase 1 - either we time out or someone get the answer right
        var answerFound = await tcs.Task;

        if (answerFound)
        {
            // Phase 2 - 1 second grace period to account for lag when multiple people get the answer right
            phase1 = false;
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        
        // Stop playing the song
        await musicPlayer.StopAsync();

        return result;
    }

    /// <summary>
    /// Checks if the answer was correct and returns how many points they got.
    /// Artist correct = 1 point
    /// Song correct = 1 point
    /// Both correct = 3 points
    /// </summary>
    private int IsAnswerCorrect(string guess)
    {
        bool artistFound = false;
        bool songFound = false;
        
        
        return 0;
    }

    public void Dispose()
    {
        _subscription?.Dispose();
    }
}