using System.Reactive.Concurrency;

namespace Amccloy.MusicBot.Asp.Net.Utils.RX
{
    public interface ISchedulerFactory
    {
        IScheduler GenerateScheduler();
    }
}