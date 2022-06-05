using System.Reactive.Concurrency;

namespace Amccloy.MusicBot.Net.Utils.RX
{
    public class DefaultSchedulerFactory : ISchedulerFactory
    {
        public IScheduler GenerateScheduler() => new EventLoopScheduler();
    }
}