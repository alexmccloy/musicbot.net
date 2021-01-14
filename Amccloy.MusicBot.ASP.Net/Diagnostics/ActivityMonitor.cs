using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using DataAccessLibrary.ActivityLogging;
using DataAccessLibrary.Models;

namespace Amccloy.MusicBot.Asp.Net.Diagnostics
{
    public class ActivityMonitor : IActivityMonitor
    {
        private readonly IActivityData _activityDatabase;
        private readonly Subject<Activity> _dbUpdates = new();

        public ActivityMonitor(IActivityData activityDatabase)
        {
            _activityDatabase = activityDatabase;
        }

        public async Task LogActivity(Activity activity)
        {
            await _activityDatabase.AddActivity(activity);
            _dbUpdates.OnNext(activity);
        }

        public IObservable<Activity> DatabaseUpdated => _dbUpdates.AsObservable();
    }
}