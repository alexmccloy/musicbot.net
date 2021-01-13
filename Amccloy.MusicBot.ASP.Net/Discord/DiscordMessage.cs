using Discord.WebSocket;

namespace Amccloy.MusicBot.Asp.Net.Discord
{
    /// <summary>
    /// Internal representation of a text message in the discord chat
    /// </summary>
    public readonly struct DiscordMessage
    {
        public readonly ISocketMessageChannel Channel;
        public readonly string UserName;
        public readonly string MessageText;

        public DiscordMessage(ISocketMessageChannel channel, string userName, string messageText)
        {
            Channel = channel;
            UserName = userName;
            MessageText = messageText;
        }

        public DiscordMessage(SocketMessage socketMessage)
        {
            Channel = socketMessage.Channel;
            UserName = socketMessage.Author.Username;
            MessageText = socketMessage.Content;
        }
    }
}