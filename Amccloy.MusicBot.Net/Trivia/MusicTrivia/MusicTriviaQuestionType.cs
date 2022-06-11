namespace Amccloy.MusicBot.Net.Trivia.MusicTrivia;

public enum MusicTriviaQuestionType
{
    /// <summary>
    /// For questions that have only artist and song name
    /// </summary>
    Standard,
    
    /// <summary>
    /// For questions that are cover song. Guess the artist, song name, and original artist
    /// </summary>
    Cover,
}