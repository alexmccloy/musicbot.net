using System.Threading.Tasks;
using Discord.WebSocket;

namespace Amccloy.MusicBot.Net.Commands
{
    public interface IDiscordCommand
    {
        /// <summary>
        /// The string that should be typed to run this command
        /// </summary>
        string CommandString { get; }

        /// <summary>
        /// Execute the discord command
        /// </summary>
        /// <param name="discordInterface"></param>
        /// <param name="args"></param>
        /// <param name="rawMessage"></param>
        /// <returns></returns>
        Task Execute(IDiscordInterface discordInterface, string[] args, SocketMessage rawMessage);
    }
}