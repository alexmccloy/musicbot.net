using System.Reactive.Concurrency;

namespace Amccloy.MusicBot.Net.Utils.RX
{
    public interface ISchedulerFactory
    {
        IScheduler GenerateScheduler();
    }
}