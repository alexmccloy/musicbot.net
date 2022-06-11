using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amccloy.MusicBot.Net.Trivia.MusicTrivia;

public interface IMusicTriviaQuestionProvider
{
    /// <summary>
    /// The names of the music trivia playlists this provider can provide. These names will be presented to the
    /// use when selecting their game
    /// </summary>
    string[] Names { get; }
    
    TimeSpan QuestionDuration { get; }

    Task<List<IMusicTriviaQuestion>> GetQuestions(int count);
}