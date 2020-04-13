using System.Reactive.Concurrency;

namespace Amccloy.MusicBot.Net.Test.Commands
{
    public class DummySchedulerFactory : ISchedulerFactory
    {
        public IScheduler GenerateScheduler() => ImmediateScheduler.Instance;
    }
}