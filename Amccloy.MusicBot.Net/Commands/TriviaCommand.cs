using System.Collections.Generic;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Discord;
using Discord.WebSocket;

namespace Amccloy.MusicBot.Net.Commands
{
    public class TriviaCommand : BaseDiscordCommand
    {
        public override string CommandString => "trivia";
        protected override string SummaryHelpText => "Plays an interactive game of trivia";

        protected override string FullHelpText {
            get { return "Placeholder text"; }
        }
        
        public override Task Execute(IDiscordInterface discordInterface, string[] args, SocketMessage rawMessage)
        {
            throw new System.NotImplementedException();
        }

        public override async Task Init()
        {
            //TODO     find all the ITriviaQuestionProviders and populate a list so that it can be used in help text and to 
            //TODO     select which source to use when playing
        }
    }
}