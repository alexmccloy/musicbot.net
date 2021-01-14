using System;

namespace DataAccessLibrary.Models
{
    /// <summary>
    /// An action or activity that the bot has done that needs to be logged
    /// </summary>
    public class Activity
    {
        public Activity()
        {
            OccuredAt = DateTime.Now;
        }
        
        public DateTime OccuredAt { get; }
        public string CommandName { get; set; }
        public string Author { get; set; }
        public string Channel { get; set; }
        public bool Succeeded { get; set; }
        public string Result { get; set; }
    }
}