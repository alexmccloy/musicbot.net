using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Utils.RX;
using NLog;

namespace Amccloy.MusicBot.Net.Trivia
{
    /// <summary>
    /// TODO make this a base provider with 1 provider per table
    /// </summary>
    public class SqliteTriviaQuestionProvider : ITriviaQuestionProvider, IDisposable
    {
        private readonly ISchedulerFactory _schedulerFactory;
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        
        public string Name => "open_trivia_db";
        public string Description => "A database of crowd sources trivia questions. Be prepared for some WEIRD SHIT";
        public TimeSpan QuestionDuration => TimeSpan.FromSeconds(30);


        public SqliteTriviaQuestionProvider(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        public Task Init()
        {
            return Task.CompletedTask;
        }

        public async Task<List<ITriviaQuestion>> GetQuestions(int count)
        {
            await using var connection = new SQLiteConnection("Data Source=Data/trivia.db;Version=3");
            var questions = new List<ITriviaQuestion>();
            string sqlQuery = $"SELECT * FROM standard_questions ORDER BY RANDOM() LIMIT {count};";

            try
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = sqlQuery;
                var dataReader = await command.ExecuteReaderAsync();

                while (dataReader.Read())
                {
                    var category = dataReader.GetString(1);
                    var question = dataReader.GetString(2);
                    var answer = dataReader.GetString(3);
                    questions.Add(new StandardTriviaQuestion(question, answer, _schedulerFactory));
                }

                return questions;
            }
            catch (Exception e)
            {
                //TODO do something useful here
                _logger.Error(e, $"Failed to open database connection: {e.Message}");
                throw;
            }

        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}