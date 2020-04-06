using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Commands;
using Amccloy.MusicBot.Net.Discord;

namespace Amccloy.MusicBot.Net.Trivia
{
    /// <summary>
    /// Represents a trivia question with a single question and single text based answer.
    /// Eg. Question: What is the capital city of Australia
    ///       Answer: Australia
    /// </summary>
    public class StandardTriviaQuestion : ITriviaQuestion
    {
        private readonly ISchedulerFactory _schedulerFactory;
        public TriviaQuestionType QuestionType => TriviaQuestionType.Standard;
        public string Question { get; }
        public string Answer { get; }

        private string _parsedAnswer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="question"></param>
        /// <param name="answer"></param>
        /// <param name="schedulerFactory"></param>
        public StandardTriviaQuestion(string question, string answer, ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
            Question = question;
            Answer = answer;
            _parsedAnswer = answer.ToLower(); //TODO strip non ascii characters from this 
        }

        /// <summary>
        /// Listen to messages coming from users, and end as soon as someone types the correct answer, or the question
        /// times out.
        /// When someone gets the correct answer, has a 1 second grace period so that multiple people guessing the
        /// answer at the same time will all get points.
        /// </summary>
        /// <param name="chat"></param>
        /// <param name="maxDuration"></param>
        /// <returns>A list of users and how many points they got, or null if no one was correct</returns>
        public async Task<List<TriviaQuestionResult>?> ExecuteQuestion(IObservable<DiscordMessage> chat, TimeSpan maxDuration)
        {
            var result = new List<TriviaQuestionResult>();
            
            // Subscribe to incoming answers from users
            // Compare answers to the actual answer, if one is correct then collect answers for 1 more second, then
            // unsubscribe and calculate scores

            return result.Count > 0 ? result : null;
        }
    }
}