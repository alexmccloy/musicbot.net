using System;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Dbo;
using Amccloy.MusicBot.Net.Discord;
using Lavalink4NET.Player;

namespace Amccloy.MusicBot.Net.Trivia.MusicTrivia;

/// <summary>
/// A single Music trivia question, where a song will play and everyone has to guess it
/// </summary>
public interface IMusicTriviaQuestion
{
    MusicTriviaQuestionType QuestionType { get; }
    
    /// <summary>
    /// The instruction that should be displayed to the user about how to answer this question.
    /// Eg. "Guess the artist and the song"
    /// </summary>
    string Instruction { get; }
    
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