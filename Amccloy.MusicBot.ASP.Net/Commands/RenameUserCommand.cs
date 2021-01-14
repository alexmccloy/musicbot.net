using System;
using System.Linq;
using System.Threading.Tasks;
using Amccloy.MusicBot.Asp.Net.Diagnostics;
using Amccloy.MusicBot.Asp.Net.Discord;
using Amccloy.MusicBot.Asp.Net.Utils.RX;
using DataAccessLibrary.Models;
using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.VisualBasic.CompilerServices;
using NLog;

namespace Amccloy.MusicBot.Asp.Net.Commands
{
    
    public class RenameUserCommand : BaseDiscordCommand
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        
        public override string CommandString => "rename";
        protected override string SummaryHelpText => "Rename a user on this server";

        protected override string FullHelpText => "Changes a users nickname on this server. Syntax:\n" +
                                                  "rename <discord name> <new name>\n" +
                                                  "put quotes around the names if there are spaces";

        protected override string[] AllowedChannels { get; } = {"robotics", "dev", "dev2"}; //TODO set this up via the web interface?
        protected override string[] AllowedRoles { get; } = {"admin", "TBD", "Battle Cup"};

        public RenameUserCommand(ISchedulerFactory schedulerFactory, IActivityMonitor activityMonitor)
            : base(schedulerFactory, activityMonitor)
        {
        }

        protected override async Task Execute(IDiscordInterface discordInterface, string[] args, SocketMessage rawMessage)
        {
            // Get the current username. This can be either the current nickname, or the discord username, or the discord user id (if it is an integer)
            // Can also be surround in quotes fuck my life

            args = Utils.ArgumentParseUtils.ParseArgsWithQuotes(args);

            if (args.Length < 3)
            {
                await SendMessage("Read how to use this command you peppeg");
                _logger.Debug($"Received invalid Rename user: {String.Join(',', args)}");
                return;
            }

            var currentName = args[1];
            var newName = args[2];

            SocketGuildUser user = null;
            
            // Try with user ID
            if (long.TryParse(currentName, out long userId))
            {
                try
                {
                    user = discordInterface.RawClient.GetUser((ulong) userId) as SocketGuildUser;
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"Cannot find user with id {userId}: {e.Message}");
                }
            }
            
            // Try with nickname or username
            if (user == null)
            {
                foreach (var guild in discordInterface.RawClient.Guilds)
                {
                    user = guild.Users.FirstOrDefault(p => p.Username == currentName || p.Nickname == currentName);
                    if (user != null) break;
                }

            }

            if (user != null)
            {
                try
                {
                    await user?.ModifyAsync(x => x.Nickname = newName);
                    await SendMessage($"Changed {currentName} to {newName}");
                    await ActivityMonitor.LogActivity(new Activity()
                    {
                        CommandName = CommandString,
                        Author = rawMessage.Author.Username,
                        Channel = rawMessage.Channel.Name,
                        Succeeded = true,
                        Result = $"Changed user {user.Username} nickname from {currentName} to {newName}"
                    });
                }
                catch (Exception e)
                {
                    await SendMessage($"ERROR: {e.Message}");
                    await ActivityMonitor.LogActivity(new Activity()
                    {
                        CommandName = CommandString,
                        Author = rawMessage.Author.Username,
                        Channel = rawMessage.Channel.Name,
                        Succeeded = false,
                        Result = $"Failed to change username for user {user.Username}: {e.Message}"
                    });
                }
            }
            else
            {
                await SendMessage($"Cannot find user with name {currentName}");
                await ActivityMonitor.LogActivity(new Activity()
                {
                    CommandName = CommandString,
                    Author = rawMessage.Author.Username,
                    Channel = rawMessage.Channel.Name,
                    Succeeded = false,
                    Result = $"Cannot find user with name {currentName}"
                });
            }

            async Task SendMessage(string message) => await discordInterface.SendMessageAsync(rawMessage.Channel, message);
        }
        
    }
}