using System.Reactive.Concurrency;
using Amccloy.MusicBot.Asp.Net.Utils.RX;

namespace Amccloy.MusicBot.Net.Test.Commands
{
    public class DummySchedulerFactory : ISchedulerFactory
    {
        public IScheduler GenerateScheduler() => ImmediateScheduler.Instance;
    }
}