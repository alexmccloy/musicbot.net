using System.Threading.Tasks;
using Discord.WebSocket;

namespace Amccloy.MusicBot.Net.Commands
{
    public class TestCommand : IDiscordCommand
    {
        public string CommandString => "test";
        
        public async Task Execute(IDiscordInterface discordInterface, string[] args, SocketMessage rawMessage)
        {
            await discordInterface.SendMessageAsync(rawMessage.Channel, "yeah boi");
        }
    }
}