using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amccloy.MusicBot.Net.Trivia
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
            foreach (var score in other.Scores)
            {
                if (Scores.ContainsKey(score.Key))
                {
                    Scores[score.Key] += score.Value;
                }
                else
                {
                    Scores.Add(score.Key, score.Value);
                }
            }
        }

        /// <summary>
        /// Returns a string representation of this score to be displayed to the users
        /// </summary>
        public string PrintScores()
        {
            if (Scores.Count == 0)
            {
                return "No one has any points!";
            }
            
            var sb = new StringBuilder();
            foreach (var pair in Scores.OrderByDescending(x => x.Value))
            {
                sb.AppendLine($"{pair.Key} - {pair.Value}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the winner or winners. This text will be displayed directly to the user
        /// TODO this is not working and prints everyones score somehow
        /// </summary>
        public string GetWinner()
        {
            if (Scores.Count == 0)
            {
                return "No one wins!";
            }

            var winners = new List<string>();
            int topScore = Int32.MinValue;
            foreach (var score in Scores.OrderByDescending(x => x.Value))
            {
                if (score.Value >= topScore)
                {
                    winners.Add(score.Key);
                }
                else
                {
                    break;
                }
            }

            return String.Join(", ", winners);
        }
    }
}