using TriviaImporter;

namespace Amccloy.MusicBot.Net.Dbo;

public class StandardTriviaQuestionDbo
{
    public int Id { get; set; }
    public TriviaCategory Category { get; set; } 
    public string Question { get; set; }
    public string Answer { get; set; }
    
    public TriviaSource Source { get; set; }
}