using System;
using System.Threading.Tasks;
using DataAccessLibrary.Models;

namespace Amccloy.MusicBot.Asp.Net.Diagnostics
{
    public interface IActivityMonitor
    {
        Task LogActivity(Activity activity);
        
        /// <summary>
        /// Allows an observer to get all changes to the database without needing to requery it
        /// </summary>
        IObservable<Activity> DatabaseUpdated { get; }
    }
}