namespace Amccloy.MusicBot.Net.Commands
{
    /// <summary>
    /// The result from a single trivia question
    /// </summary>
    public readonly struct TriviaQuestionResult
    {
        public readonly string UserName;
        public readonly int Points;

        public TriviaQuestionResult(string userName, int points)
        {
            UserName = userName;
            Points = points;
        }
    }
}