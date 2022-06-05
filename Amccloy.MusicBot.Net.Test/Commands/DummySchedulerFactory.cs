using System.Reactive.Concurrency;
using Amccloy.MusicBot.Net.Utils.RX;

namespace Amccloy.MusicBot.Net.Test.Commands
{
    public class DummySchedulerFactory : ISchedulerFactory
    {
        public IScheduler GenerateScheduler() => ImmediateScheduler.Instance;
    }
}