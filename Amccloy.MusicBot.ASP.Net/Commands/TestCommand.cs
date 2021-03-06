﻿using System.Text;
using System.Threading.Tasks;
using Amccloy.MusicBot.Asp.Net.Diagnostics;
using Amccloy.MusicBot.Asp.Net.Discord;
using Amccloy.MusicBot.Asp.Net.Utils.RX;
using Discord.WebSocket;

namespace Amccloy.MusicBot.Asp.Net.Commands
{
    public class TestCommand : BaseDiscordCommand
    {
        public override string CommandString => "test";
        
        protected override string SummaryHelpText => "Will respond with all details and arguments it was given";

        protected override string FullHelpText => "Full help text for Test Command.\n" +
                                      "It will reply with the details of the person that ran it, and all the arguments " +
                                      "it received.";

        public TestCommand(ISchedulerFactory schedulerFactory, IActivityMonitor activityMonitor)
            : base(schedulerFactory, activityMonitor)
        {
        }

        protected override async Task Execute(IDiscordInterface discordInterface, string[] args, SocketMessage rawMessage)
        {
            var response = new StringBuilder();

            response.AppendLine("Test command:");
            response.AppendLine($"From User: {rawMessage.Author.Username}");
            response.AppendLine($"Channel: {rawMessage.Channel.Name}");

            for (int i = 0; i < args.Length; i++)
            {
                response.AppendLine($"Arg{i}: {args[i]}");
            }
            
            await discordInterface.SendMessageAsync(rawMessage.Channel, response.ToString());
        }

    }
}