using System.Threading.Tasks;
using Amccloy.MusicBot.Asp.Net.Discord;
using Amccloy.MusicBot.Asp.Net.Utils.RX;
using Discord.WebSocket;

namespace Amccloy.MusicBot.Asp.Net.Commands
{
    /// <summary>
    /// The base class for a command that the user will type into discord
    /// </summary>
    public abstract class BaseDiscordCommand
    {
        protected readonly ISchedulerFactory SchedulerFactory;

        protected BaseDiscordCommand(ISchedulerFactory schedulerFactory)
        {
            SchedulerFactory = schedulerFactory;
        }

        /// <summary>
        /// The string that should be typed to run this command
        /// </summary>
        public abstract string CommandString { get; }
        
        /// <summary>
        /// A short (one line) summary of what this command does
        /// </summary>
        protected abstract string SummaryHelpText { get; }

        /// <summary>
        /// A detailed summary of what this command does and its possible arguments
        /// </summary>
        protected abstract string FullHelpText { get; }

        /// <summary>
        /// Gets the Summary help string that should be printed by the !help command
        /// </summary>
        public string PrintSummaryHelpText() => $"{CommandString}: {SummaryHelpText}";

        /// <summary>
        /// Gets the full help string that should be printed by the !help command
        /// </summary>
        public string PrintFullHelpText() => $"Command: {CommandString}\n" +
                                             $"{FullHelpText}";

        /// <summary>
        /// Execute the discord command
        /// </summary>
        /// <param name="discordInterface"></param>
        /// <param name="args"></param>
        /// <param name="rawMessage"></param>
        /// <returns></returns>
        public abstract Task Execute(IDiscordInterface discordInterface, string[] args, SocketMessage rawMessage);

        /// <summary>
        /// Perform any initialisation required here
        /// </summary>
        public virtual Task Init() => Task.CompletedTask;
    }
}