using System.Collections.Generic;

namespace Amccloy.MusicBot.Net.Commands
{
    /// <summary>
    /// The result from a single trivia question
    /// </summary>
    public class GameResults
    {
        /// <summary>
        /// List of scores with the players user name as the key
        /// </summary>
        public Dictionary<string, int> Scores { get; private set; } = new Dictionary<string, int>();

        /// <summary>
        /// Adds a score to the players existing score, or creates a new row if that player does not exist yet
        /// </summary>
        /// <param name="username"></param>
        /// <param name="score"></param>
        public void AddScore(string username, int score)
        {
            if (Scores.ContainsKey(username))
            {
                Scores[username] += score;
            }
            else
            {
                Scores.Add(username, score);
            }
        }

        /// <summary>
        /// Adds another score to this one
        /// </summary>
        /// <param name="other"></param>
        public void CombineWith(GameResults other)
        {
            
        }
    }
}