using System.Reactive.Concurrency;

namespace Amccloy.MusicBot.Net
{
    public class DefaultSchedulerFactory : ISchedulerFactory
    {
        public IScheduler GenerateScheduler() => new EventLoopScheduler();
    }
}