using System.Reactive.Concurrency;

namespace Amccloy.MusicBot.Asp.Net.Utils.RX
{
    public class DefaultSchedulerFactory : ISchedulerFactory
    {
        public IScheduler GenerateScheduler() => new EventLoopScheduler();
    }
}