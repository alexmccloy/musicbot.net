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

        public TestTriviaQuestionProvider(ISchedulerFactory schedulerFactoryFactory)
        {
            _schedulerFactory = schedulerFactoryFactory;
        }

        public List<ITriviaQuestion> GetQuestions(int count)
        {
            var questions = new List<ITriviaQuestion>();
            for (int i = 0; i < count; i++)
            {
                questions.Add(GenerateStandardTriviaQuestion());
            }

            return questions;
        }

        private ITriviaQuestion GenerateStandardTriviaQuestion() => 
            new StandardTriviaQuestion($"Question {_count}", $"Answer {_count++}", _schedulerFactory);
    }
}