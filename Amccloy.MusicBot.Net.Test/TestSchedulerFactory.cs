using System.Collections.Generic;
using System.Reactive.Concurrency;
using Amccloy.MusicBot.Net.Utils.RX;
using Microsoft.Reactive.Testing;

namespace Amccloy.MusicBot.Net.Test
{
    /// <summary>
    /// Scheduler factory that returns TestSchedulers and adds all generated schedulers to a list so they can be
    /// retrieved later
    /// </summary>
    public class TestSchedulerFactory : ISchedulerFactory
    {
        public List<TestScheduler> Schedulers { get; } = new List<TestScheduler>();
        
        public IScheduler GenerateScheduler()
        {
            
            var scheduler = new TestScheduler();
            Schedulers.Add(scheduler);
            return scheduler;
        }
    }
}