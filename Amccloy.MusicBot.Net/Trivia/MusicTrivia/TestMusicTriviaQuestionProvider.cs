using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Dbo;
using Amccloy.MusicBot.Net.Utils.RX;
using Lavalink4NET;

namespace Amccloy.MusicBot.Net.Trivia.MusicTrivia;

public class TestMusicTriviaQuestionProvider : IMusicTriviaQuestionProvider
{
    public string[] Playlists => new[] {"test"};

    public string Source => "Test";

    public TimeSpan QuestionDuration => TimeSpan.FromSeconds(30);

    // Dependencies of Music Trivia Questions
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IAudioService _audioService;
    
    public TestMusicTriviaQuestionProvider(ISchedulerFactory schedulerFactory, IAudioService audioService)
    {
        _schedulerFactory = schedulerFactory;
        _audioService = audioService;
    }

    public Task<List<IMusicTriviaQuestion>> GetQuestions(int count)
    {
        var questions = new List<IMusicTriviaQuestion>();
        foreach (Song song in _songs)
        {
            questions.Add(new MusicTriviaQuestion(song, _schedulerFactory, _audioService));
        }

        return Task.FromResult(questions);
    }

    private readonly List<Song> _songs = new List<Song>()
    {
        new Song() {Artist = "AC/DC", Name = "Back in black"},
        new Song() {Artist = "AC/DC", Name = "Thunderstruck"},
        new Song() {Artist = "Metallica", Name = "Enter Sandman"},
        new Song() {Artist = "Alter Bridge", Name = "Watch Over You"},
    };


}