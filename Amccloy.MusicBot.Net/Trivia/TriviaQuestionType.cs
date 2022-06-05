namespace Amccloy.MusicBot.Net.Trivia
{
    public enum TriviaQuestionType
    {
        /// <summary>
        /// A question with a single answer
        /// </summary>
        Standard,
        
        /// <summary>
        /// A question with multiple possible answers and the user must select one
        /// </summary>
        MultiChoice,
        
        /// <summary>
        /// A question where there are multiple correct answers that are ranked based on popularity
        /// </summary>
        FamilyFeud,
    }
}