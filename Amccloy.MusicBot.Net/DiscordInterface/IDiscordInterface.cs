using System;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.WebSocket;

namespace Amccloy.MusicBot.Net
{
    public interface IDiscordInterface
    {
        public IObservable<SocketMessage> MessageReceived { get; }
        public Task SendMessageAsync(ISocketMessageChannel channel, string message);
        // public Task SendMessageAsync(int channelId, string message);

        Task Init();
        Task Stop();
    }
}