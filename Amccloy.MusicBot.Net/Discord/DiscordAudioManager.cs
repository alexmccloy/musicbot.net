using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amccloy.MusicBot.Net.Commands;
using Lavalink4NET;
using Lavalink4NET.Player;
using Microsoft.Extensions.Hosting;

namespace Amccloy.MusicBot.Net.Discord;

/// <summary>
/// Class to make sure the audio stops correctly when the application is shutting down
/// </summary>
public class DiscordAudioManager : BackgroundService
{
    private static IDiscordInterface _discordInterface;
    private static IAudioService _audioService;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public DiscordAudioManager(IDiscordInterface discordInterface, 
                               IAudioService audioService, 
                               IHostApplicationLifetime applicationLifetime)
    {
        _discordInterface = discordInterface;
        _audioService = audioService;
        _applicationLifetime = applicationLifetime;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _applicationLifetime.ApplicationStopping.Register(() =>
        {
            _audioService.Dispose();
            _discordInterface.RawClient.Dispose();
        });
        
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Gets the music player, and if necessary joins the channel of the user that requested it
    /// </summary>
    /// TODO accessing this statically is probs bad, it should be a wrapper that can be injected
    public static async Task<LavalinkPlayer> GetPlayer(ulong authorId)
    {
        var guild = _discordInterface.RawClient.Guilds.First();
        var player = _audioService.GetPlayer<LavalinkPlayer>(guild.ApplicationId ?? default);

        // Check if the player already exists and is connected
        if (player != null && player.State != PlayerState.NotConnected && player.State != PlayerState.Destroyed)
        {
            return player;
        }

        // Check if the user that sent the command is in a voice channel
        var userId = authorId;
        var user = guild.Users.First(u => u.Id == authorId);
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