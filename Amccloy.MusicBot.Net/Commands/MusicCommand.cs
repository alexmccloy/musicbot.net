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

    protected override async Task Execute(IDiscordInterface discordInterface, string[] args, SocketMessage commandMessage)
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
                await PlayTrack(args[2..], discordInterface, commandMessage);
                break;
            case "stop":
                await StopPlaying(discordInterface, commandMessage);
                break;
            default:
                await SendMessage($"Unknown command: {args[1]}");
                break;
        }
        
        
        async Task SendMessage(string message) => await discordInterface.SendMessageAsync(commandMessage.Channel, message);
    }

    private async Task PlayTrack(string[] searchTerms, IDiscordInterface discordInterface, SocketMessage rawMessage)
    {
        var query = string.Join(' ', searchTerms);
        _logger.Debug($"Playing track with search terms: {query}");
        
        try
        {
            var player = await DiscordAudioManager.GetPlayer(rawMessage.Author.Id);

            var track = await _audioService.GetTrackAsync(query, SearchMode.YouTube) 
                     ?? throw new DiscordCommandException("Search returned no results");

            await player.PlayAsync(track);

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
            var player = await DiscordAudioManager.GetPlayer(rawMessage.Author.Id);

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


}