using System;

namespace Amccloy.MusicBot.Net.Commands;

public class DiscordCommandException : Exception
{
    public DiscordCommandException(string reason) : base(reason)
    {
    }
}