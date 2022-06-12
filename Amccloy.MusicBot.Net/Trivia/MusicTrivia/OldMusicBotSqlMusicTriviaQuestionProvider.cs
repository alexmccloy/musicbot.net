using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Dbo;
using Amccloy.MusicBot.Net.Model;
using Amccloy.MusicBot.Net.Utils;
using Amccloy.MusicBot.Net.Utils.RX;
using Lavalink4NET;
using Microsoft.EntityFrameworkCore;

namespace Amccloy.MusicBot.Net.Trivia.MusicTrivia;

public class OldMusicBotSqlMusicTriviaQuestionProvider : IMusicTriviaQuestionProvider
{
    private readonly IDbContextFactory<TriviaContext> _dbContextFactory;
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IAudioService _audioService;
    public string[] Playlists => GetPlaylists();
    private string[] _playlists = null;
    
    public string Source => "OldMusicBot";
    public TimeSpan QuestionDuration => TimeSpan.FromSeconds(30);

    public OldMusicBotSqlMusicTriviaQuestionProvider(IDbContextFactory<TriviaContext> dbContextFactory,
                                                     ISchedulerFactory schedulerFactory,
                                                     IAudioService audioService)
    {
        _dbContextFactory = dbContextFactory;
        _schedulerFactory = schedulerFactory;
        _audioService = audioService;
    }

    public async Task<List<IMusicTriviaQuestion>> GetQuestions(int count)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();

        // Get some random songs
        var songs = await dbContext.MusicTriviaQuestions
                                       .OrderBy(x => Guid.NewGuid())
                                       .Take(count)
                                       .AsNoTracking()
                                       .ToListAsync();

        return songs.Select(song => new MusicTriviaQuestion(song.Song, _schedulerFactory, _audioService) as IMusicTriviaQuestion)
                    .ToList();
    }

    private string[] GetPlaylists()
    {
        // Lazy load playlists
        if (_playlists != null) return _playlists;

        var dbContext = _dbContextFactory.CreateDbContext();
        var playlists = dbContext.MusicTriviaQuestions
                                 .DbDistinctBy(q => q.Playlist)
                                 .AsNoTracking()
                                 .ToList();

        return playlists.Select(s => s.Playlist).ToArray();

    }
}