namespace Amccloy.MusicBot.Net.Dbo;

public class MusicTriviaQuestionDbo
{
    public int Id { get; set; }
    
    /// <summary>
    /// Where this playlist came from
    /// </summary>
    public MusicTriviaSource Source { get; set; }
    
    /// <summary>
    /// The playlist this question belongs to
    /// </summary>
    public string Playlist { get; set; }
    
    public Song Song { get; set; }
}

public enum MusicTriviaSource
{
    /// <summary>
    /// The old python music bot
    /// </summary>
    OldMusicBot = 1,
}