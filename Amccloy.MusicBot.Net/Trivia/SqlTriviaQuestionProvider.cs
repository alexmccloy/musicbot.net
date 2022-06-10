using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Dbo;
using Amccloy.MusicBot.Net.Model;
using Amccloy.MusicBot.Net.Utils.RX;
using Microsoft.EntityFrameworkCore;

namespace Amccloy.MusicBot.Net.Trivia;

public class SqlTriviaQuestionProvider : ITriviaQuestionProvider
{
    private readonly IDbContextFactory<TriviaContext> _dbContextFactory;
    private readonly ISchedulerFactory _schedulerFactory;
    public string Name => "open_trivia_db";
    public string Description => "A database of crowd sources trivia questions. Be prepared for some WEIRD SHIT";
    public TimeSpan QuestionDuration => TimeSpan.FromSeconds(30);

    public SqlTriviaQuestionProvider(IDbContextFactory<TriviaContext> dbContextFactory,
                                     ISchedulerFactory schedulerFactory)
    {
        _dbContextFactory = dbContextFactory;
        _schedulerFactory = schedulerFactory;
    }

    public Task Init()
    {
        return Task.CompletedTask;
    }

        
    public async Task<List<ITriviaQuestion>> GetQuestions(int count)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();

        // Return some random questions
        var questions = await dbContext.StandardTriviaQuestions
                                       .OrderBy(x => Guid.NewGuid())
                                       .Take(count)
                                       .AsNoTracking()
                                       .ToListAsync();

        return questions.Select(q => new StandardTriviaQuestion(q.Question, q.Answer, _schedulerFactory) as ITriviaQuestion)
                        .ToList();
    }
}