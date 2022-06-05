using System;
using System.Linq;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Discord;
using Amccloy.MusicBot.Net.Utils;
using Amccloy.MusicBot.Net.Utils.RX;
using Discord.WebSocket;
using NLog;

namespace Amccloy.MusicBot.Net.Commands
{
    /// <summary>
    /// The base class for a command that the user will type into discord
    /// </summary>
    public abstract class BaseDiscordCommand
    {
        protected readonly ISchedulerFactory SchedulerFactory;
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

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
        /// The channels this command is allowed to be used in. Blank for all channels
        /// </summary>
        protected virtual string[] AllowedChannels { get; } = {};

        /// <summary>
        /// Whitelist for roles that are allowed to execute this command. An empty list means all roles are allowed
        /// </summary>
        protected virtual string[] AllowedRoles { get; } = {};
        
        /// <summary>
        /// Blacklist for roles that are allowed to execute this command. An empty list means no roles are banned.
        /// </summary>
        protected virtual string[] DisallowedRoles { get; } = {};

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
        protected abstract Task Execute(IDiscordInterface discordInterface, string[] args, SocketMessage rawMessage);

        /// <summary>
        /// Perform any initialisation required here
        /// </summary>
        public virtual Task Init() => Task.CompletedTask;

        public async Task ExecuteCommand(IDiscordInterface discordInterface, string[] args, SocketMessage rawMessage)
        {
            try
            {
                bool allowed = true;
                string reason = "";

                // Check if we are allowed to do this command in this channel
                if (AllowedChannels.Length != 0 && !AllowedChannels.Contains(rawMessage.Channel.Name))
                {
                    allowed = false;
                    reason += $"In channel {rawMessage.Channel.Name} but allowed channels are {String.Join(',', AllowedChannels)}\n";
                }

                // Check if this user has a role that is in the whitelist
                if (AllowedRoles.Length != 0 && !rawMessage.Author.GetRolesAsString().ToList().Intersect(AllowedRoles).Any())
                {
                    allowed = false;
                    reason += $"Allowed roles are {String.Join(',', AllowedRoles)} but user {rawMessage.Author.Username} " +
                              $"only has roles {String.Join(',', rawMessage.Author.GetRolesAsString())}\n";
                }
                
                // Check if this user has a role that is in the blacklist
                if (DisallowedRoles.Length != 0 && rawMessage.Author.GetRolesAsString().Intersect(DisallowedRoles).Any())
                {
                    allowed = false;
                    reason += $"User {rawMessage.Author.Username} has blacklisted role/s " +
                              $"{String.Join(',', rawMessage.Author.GetRolesAsString().Intersect(DisallowedRoles))}\n";
                }

                if (allowed)
                {
                    await Execute(discordInterface, args, rawMessage);
                }
                else
                {
                    _logger.Warn($"User {rawMessage.Author.Username} tried to execute command {CommandString} but it failed because:\n{reason.Trim()}");
                }

            }
            catch (Exception e)
            {
                _logger.Error(e, $"Unexpected error when attmpeting to exectue a command: {e.Message}");
            }
        }
    }
}