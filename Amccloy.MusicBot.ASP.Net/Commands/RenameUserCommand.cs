using System.Threading.Tasks;
using Amccloy.MusicBot.Asp.Net.Discord;
using Amccloy.MusicBot.Asp.Net.Utils.RX;
using Discord.WebSocket;

namespace Amccloy.MusicBot.Asp.Net.Commands
{
    
    public class RenameUserCommand : BaseDiscordCommand
    {
        public override string CommandString => "rename";
        protected override string SummaryHelpText => "Rename a user on this server";

        protected override string FullHelpText => "Changes a users nickname on this server. Syntax:\n" +
                                                  "rename <discord name> <new name>\n" +
                                                  "put quotes around the names if there are spaces";
        
        public RenameUserCommand(ISchedulerFactory schedulerFactory)
            : base(schedulerFactory)
        {
        }

        public override async Task Execute(IDiscordInterface discordInterface, string[] args, SocketMessage rawMessage)
        {
            // Get the current username. This can be either the current nickname, or the discord username, or the discord user id (if it is an integer)
            // Can also be surround in quotes fuck my life
            
            
        }
        
    }
}