using System;
using System.Collections.Generic;
using Discord.WebSocket;

namespace Amccloy.MusicBot.Asp.Net.Utils
{
    public static class DiscordExtensions
    {
        public static IEnumerable<String> GetRolesAsString(this SocketUser user)
        {
            foreach (var role in GetRoles(user))
            {
                yield return role.Name;
            }
        }

        public static IEnumerable<SocketRole> GetRoles(this SocketUser user)
        {
            if (user is SocketGuildUser guildUser)
            {
                return guildUser.Roles;
            }
            else
            {
                throw new ApplicationException($"Cannot convert {nameof(SocketUser)} to {nameof(SocketGuildUser)} for some reason...");
            }
        }
    }
}