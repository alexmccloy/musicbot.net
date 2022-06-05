using System;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Amccloy.MusicBot.Net.Discord
{
    public interface IDiscordInterface
    {
        public IObservable<SocketMessage> MessageReceived { get; }
        public Task SendMessageAsync(ISocketMessageChannel channel, string message);

        public DiscordSocketClient RawClient { get; }
    }
}