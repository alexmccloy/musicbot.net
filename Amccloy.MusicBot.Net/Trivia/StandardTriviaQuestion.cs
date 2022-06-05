using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Discord;
using Amccloy.MusicBot.Net.Utils.RX;

namespace Amccloy.MusicBot.Net.Trivia
{
    /// <summary>
    /// Represents a trivia question with a single question and single text based answer.
    /// Eg. Question: What is the capital city of Australia
    ///       Answer: Canberra
    /// </summary>
    public class StandardTriviaQuestion : ITriviaQuestion, IDisposable
    {
        private readonly IScheduler _scheduler;
        private IDisposable _subscription;
        public TriviaQuestionType QuestionType => TriviaQuestionType.Standard;
        public string Question { get; }
        public string Answer { get; }

        /// <summary>
        /// How many points this question is worth for getting it correct
        /// </summary>
        public int Points { get; set; } = 1;

        private readonly string _parsedAnswer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="question"></param>
        /// <param name="answer"></param>
        /// <param name="schedulerFactory"></param>
        public StandardTriviaQuestion(string question, string answer, ISchedulerFactory schedulerFactory)
        {
            _scheduler = schedulerFactory.GenerateScheduler();
            Question = question;
            Answer = answer;
            _parsedAnswer = answer.ToLower().Trim(); //TODO strip non ascii characters from this 
        }

        /// <summary>
        /// Listen to messages coming from users, and end as soon as someone types the correct answer, or the question
        /// times out.
        /// When someone gets the correct answer, has a 1 second grace period so that multiple people guessing the
        /// answer at the same time will all get points.
        /// </summary>
        /// <param name="chat"></param>
        /// <param name="maxDuration"></param>
        /// <returns>A list of users and how many points they got,</returns>
        public async Task<GameResults> ExecuteQuestion(IObservable<DiscordMessage> chat, TimeSpan maxDuration)
        {
            if (maxDuration.TotalSeconds < 2)
            {
                throw new InvalidOperationException($"The timespan must be greater than 2 seconds, but was {maxDuration.TotalSeconds} seconds");
            }
            
            var result = new GameResults();
            var waitHandle = new SemaphoreSlim(1);
            var cancellationTokenSource = new CancellationTokenSource();

            bool phase1 = true; // When this is true the round ends when the first person gets the question right,
                                // when it is false we are in the 1 second grace period

            await waitHandle.WaitAsync(); // Take the wait handle and wait for the observable to release it
            
            // Subscribe to incoming answers from users
            _subscription = chat.ObserveOn(_scheduler).Subscribe(message =>
            {
                if (IsAnswerCorrect(message.MessageText))
                {
                    // Someone was correct,
                    result.AddScore(message.UserName, Points);
                    
                    if (phase1)
                    {
                        cancellationTokenSource.Cancel();
                        waitHandle.Release();
                    }
                }
            });

            //PHASE 1 - get the first correct answer and end
            
            //Release the lock after the time has run out
            Task doAfter = TimerUtils.TimerUtils.DoAfter(() => waitHandle.Release(), 
                                                         (int) maxDuration.Subtract(TimeSpan.FromSeconds(1)).TotalMilliseconds,
                                                         cancellationTokenSource.Token);
            await waitHandle.WaitAsync();
            phase1 = false;
            
            //PHASE 2 - always lasts for 1 second and add all answers
            await Task.Delay(1);

            return result;
        }

        /// <summary>
        /// Determines if the answer from the user is correct.
        /// Non-case sensitive, white space is stripped.
        /// </summary>
        private bool IsAnswerCorrect(string answer)
        {
            answer = answer.ToLower().Trim();

            return answer == _parsedAnswer;
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}