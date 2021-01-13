using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amccloy.MusicBot.Asp.Net.Trivia
{
    /// <summary>
    /// Source of trivia questions, eg a Web API or Excel file. The trivia runner will use this to source questions
    /// when running a game.
    /// </summary>
    public interface ITriviaQuestionProvider
    {
        /// <summary>
        /// The provider should do any initialisation it requires here. This will be called before any questions are
        /// requested (but after the <see cref="Name"/> is retrieved)
        /// </summary>
        Task Init();
        
        /// <summary>
        /// The name of the provider. This will be displayed to the user when selecting what game to play
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// The description for the provider, this will also be displayed to the user
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// How long each question should last for
        /// </summary>
        TimeSpan QuestionDuration { get; } 
        
        /// <summary>
        /// Get the next <see cref="count"/> number of questions.
        /// TODO decide what to do if there arent that many available
        /// </summary>
        Task<List<ITriviaQuestion>> GetQuestions(int count);
    }
}