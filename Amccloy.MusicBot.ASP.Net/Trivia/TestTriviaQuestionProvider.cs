using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amccloy.MusicBot.Net.Trivia
{
    /// <summary>
    /// An implementation of a trivia question provider that only returns static text based on the question number
    /// </summary>
    public class TestTriviaQuestionProvider : ITriviaQuestionProvider
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private int _count = 0;

        public Task Init() => Task.CompletedTask;

        public string Name => "Test";
        public string Description => "This is a test trivia source";
        
        public TimeSpan QuestionDuration => TimeSpan.FromSeconds(30);

        public TestTriviaQuestionProvider(ISchedulerFactory schedulerFactoryFactory)
        {
            _schedulerFactory = schedulerFactoryFactory;
        }

        public Task<List<ITriviaQuestion>> GetQuestions(int count)
        {
            var questions = new List<ITriviaQuestion>();
            for (int i = 0; i < count; i++)
            {
                questions.Add(GenerateStandardTriviaQuestion());
            }

            return Task.FromResult(questions);
        }

        private ITriviaQuestion GenerateStandardTriviaQuestion() => 
            new StandardTriviaQuestion($"This is the text for question {++_count}", $"Answer {_count}", _schedulerFactory);
    }
}