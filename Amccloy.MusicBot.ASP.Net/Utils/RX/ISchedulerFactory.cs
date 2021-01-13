using System.Reactive.Concurrency;

namespace Amccloy.MusicBot.Net
{
    public interface ISchedulerFactory
    {
        IScheduler GenerateScheduler();
    }
}