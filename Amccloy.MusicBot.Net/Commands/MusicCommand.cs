using System;
using System.Linq;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Discord;
using Amccloy.MusicBot.Net.Utils.RX;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using Microsoft.VisualBasic;

namespace Amccloy.MusicBot.Net.Commands;

public class MusicCommand : BaseDiscordCommand
{
    private readonly IAudioService _audioService;
    private readonly IDiscordInterface _discordInterface;

    public override string CommandString => "music";
    protected override string SummaryHelpText => "Plays and controls music";

    protected override string FullHelpText => "Plays music on this server. Syntax:\n" +
                                              "play <song and artist name>\n" +
                                              "stop";
    
    
    public MusicCommand(ISchedulerFactory schedulerFactory, 
                        IAudioService audioService)
        : base(schedulerFactory)
    {
        _audioService = audioService;
    }

    protected override async Task Execute(IDiscordInterface discordInterface, string[] args, SocketMessage rawMessage)
    {
        if (args.Length == 1)
        {
            // The user has not put in any arguments
                await SendMessage("Read how to use this command you peppeg");
                _logger.Debug($"Received invalid play command: {string.Join(',', args)}");
                return;
        }
        
        // Determine which mode
        switch (args[1])
        {
            case "play":
                await PlayTrack(args[2..], discordInterface, rawMessage);
                break;
            case "stop":
                await StopPlaying(discordInterface, rawMessage);
                break;
            default:
                await SendMessage($"Unknown command: {args[1]}");
                break;
        }
        
        
        async Task SendMessage(string message) => await discordInterface.SendMessageAsync(rawMessage.Channel, message);
    }

    private async Task PlayTrack(string[] searchTerms, IDiscordInterface discordInterface, SocketMessage rawMessage)
    {
        var query = string.Join(' ', searchTerms);
        _logger.Debug($"Playing track with search terms: {query}");
        
        try
        {
            var player = await GetPlayer(discordInterface, rawMessage);

            var track = await _audioService.GetTrackAsync(query, SearchMode.YouTube) 
                     ?? throw new DiscordCommandException("Search returned no results");

            await player.PlayAsync(track, noReplace: true);

        }
        catch (DiscordCommandException e)
        {
            await discordInterface.SendMessageAsync(rawMessage.Channel, e.Message);
            return;
        }
    }

    private async Task StopPlaying(IDiscordInterface discordInterface, SocketMessage rawMessage)
    {
        try
        {
            var player = await GetPlayer(discordInterface, rawMessage);

            if (player.CurrentTrack == null)
            {
                return;
            }

            await player.StopAsync();
        }
        catch (DiscordCommandException e)
        {
            await discordInterface.SendMessageAsync(rawMessage.Channel, e.Message);
            return;
        }
    }

    /// <summary>
    /// Gets the music player, and if necessary joins the channel of the user that requested it
    /// </summary>
    /// TODO tidy this up so we are only sending the actual properties it requires
    private async Task<LavalinkPlayer> GetPlayer(IDiscordInterface discordInterface, SocketMessage rawMessage)
    {
        var guild = discordInterface.RawClient.Guilds.First();
        var player = _audioService.GetPlayer<LavalinkPlayer>(guild.ApplicationId ?? default);

        // Check if the player already exists and is connected
        if (player != null && player.State != PlayerState.NotConnected && player.State != PlayerState.Destroyed)
        {
            return player;
        }

        // Check if the user that sent the command is in a voice channel
        var userId = rawMessage.Author.Id;
        var user = guild.Users.First(u => u.Id == rawMessage.Author.Id);
        if (!user.VoiceState.HasValue)
        {
            throw new DiscordCommandException("The user must be in a voice channel to use this command");
        }
        
        // Join the voice channel
        var result = await _audioService.JoinAsync<LavalinkPlayer>(user.Guild.Id, user.VoiceChannel.Id);
        if (result == null)
        {
            throw new Exception("Unable to join voice channel. Who knows why pepehands");
        }

        return result;
    }
}