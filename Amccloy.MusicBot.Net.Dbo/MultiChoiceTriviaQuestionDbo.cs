using System.ComponentModel.DataAnnotations.Schema;

namespace Amccloy.MusicBot.Net.Dbo;

public class MultiChoiceTriviaQuestionDbo
{
    public int Id { get; set; }
    
    public TriviaCategory Category { get; set; }
    
    public string Question { get; set; }
    
    public string Answer { get; set; }
    
    [Column(TypeName = "jsonb")]
    public string[] FalseAnswers { get; set; }
    
    public TriviaSource Source { get; set; }
}