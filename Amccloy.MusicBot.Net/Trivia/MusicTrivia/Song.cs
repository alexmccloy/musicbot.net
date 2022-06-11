namespace Amccloy.MusicBot.Net.Trivia.MusicTrivia;

public record Song
{
    public string Name { get; set; }
    public string Artist { get; set; } //TODO should this become a list of artists or is that too complex?
    public string Album { get; set; }
    public string Year { get; set; }
    
    /// <summary>
    /// If this is a cover song then who sung the original
    /// </summary>
    public string OriginalArtist { get; set; }
}