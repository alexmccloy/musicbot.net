using System.Threading.Tasks;
using Amccloy.MusicBot.Asp.Net.Commands;
using Amccloy.MusicBot.Asp.Net.Diagnostics;
using Amccloy.MusicBot.Asp.Net.Discord;
using Amccloy.MusicBot.Asp.Net.Utils.RX;
using Discord.WebSocket;

namespace Amccloy.MusicBot.Net.Test.Commands
{
    public class DummyCommand : BaseDiscordCommand
    {
        public const string Command = "dummyCommand";
        public const string SummaryHelp = "summary_text_here";
        public const string FullHelp = "full_help_text_here";

        public int ExecuteCount = 0;

        public override string CommandString => Command;
        protected override string SummaryHelpText => SummaryHelp;
        protected override string FullHelpText => FullHelp;

        public DummyCommand(ISchedulerFactory schedulerFactory, IActivityMonitor activityMonitor)
            : base(schedulerFactory, activityMonitor)
        {
        }

        protected override Task Execute(IDiscordInterface discordInterface, string[] args, SocketMessage rawMessage)
        {
            ExecuteCount++;
            return Task.CompletedTask;
        }
    }
}