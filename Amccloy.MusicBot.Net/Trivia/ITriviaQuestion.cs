using System;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Discord;

namespace Amccloy.MusicBot.Net.Trivia
{
    /// <summary>
    /// Represents a single trivia question
    /// </summary>
    public interface ITriviaQuestion
    {
        TriviaQuestionType QuestionType { get; }
        
        /// <summary>
        /// The question. This is what will be displayed to the user before the <see cref="ExecuteQuestion"/> method is
        /// called
        /// </summary>
        string Question { get; }
        
        /// <summary>
        /// The correct answer to the question. This is what will be displayed after the <see cref="ExecuteQuestion"/>
        /// method. It does not need to be used by the execute method to actually determine the correct answer.
        /// </summary>
        string Answer { get; }

        /// <summary>
        /// This method should run the entire question. It will decide whether the question should be time based or end
        /// when the first person gets it correct. It can interact with the user (eg. to provide hints).
        /// When the question has been answered it will return a list of users and how many points the got
        /// Points return can be negative if the user should loose points
        /// </summary>
        /// <param name="chat">A feed of chat messages that are considered answers to the question</param>
        /// <param name="maxDuration">The max duration the question should run for. If the test runs for longer than
        /// this the calling method may time it out and it will not be able to return a value</param>
        /// <returns>A list of users and the points they earned or lost while answering this question</returns>
        Task<GameResults> ExecuteQuestion(IObservable<DiscordMessage> chat, TimeSpan maxDuration);
    }
}