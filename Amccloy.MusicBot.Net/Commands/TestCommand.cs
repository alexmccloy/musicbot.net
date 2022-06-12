using System.Text;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Discord;
using Amccloy.MusicBot.Net.Utils.RX;
using Discord.WebSocket;

namespace Amccloy.MusicBot.Net.Commands
{
    public class TestCommand : BaseDiscordCommand
    {
        public override string CommandString => "test";
        
        protected override string SummaryHelpText => "Will respond with all details and arguments it was given";

        protected override string FullHelpText => "Full help text for Test Command.\n" +
                                      "It will reply with the details of the person that ran it, and all the arguments " +
                                      "it received.";

        public TestCommand(ISchedulerFactory schedulerFactory)
            : base(schedulerFactory)
        {
        }

        protected override async Task Execute(IDiscordInterface discordInterface, string[] args, SocketMessage commandMessage)
        {
            var response = new StringBuilder();

            response.AppendLine("Test command:");
            response.AppendLine($"From User: {commandMessage.Author.Username}");
            response.AppendLine($"Channel: {commandMessage.Channel.Name}");

            for (int i = 0; i < args.Length; i++)
            {
                response.AppendLine($"Arg{i}: {args[i]}");
            }
            
            await discordInterface.SendMessageAsync(commandMessage.Channel, response.ToString());
        }

    }
}