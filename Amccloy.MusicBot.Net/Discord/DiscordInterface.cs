using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using NLog;

namespace Amccloy.MusicBot.Net.Discord
{
    /// <summary>
    /// Connects to the discord server and acts as the interface for sending and receiving messages
    /// </summary>
    public class DiscordInterface : IDiscordInterface
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly DiscordSocketClient _discordClient;
        private readonly Subject<SocketMessage> _socketMessageSubject = new Subject<SocketMessage>();

        private readonly string _token;

        public DiscordInterface(DiscordSocketClient discordClient,
                              IConfiguration configuration)
        {
            _discordClient = discordClient;
            _token = configuration["Discord:BotToken"];
        }

        public async Task Init()
        {
            _logger.Info("Starting Discord Client Interface");
            _discordClient.Log += HandleLogMessageAsync;
            _discordClient.Ready += HandleClientReadyAsync;
            _discordClient.MessageReceived += HandleMessageReceivedAsync;

            await _discordClient.LoginAsync(TokenType.Bot, _token);
            await _discordClient.StartAsync();
            
        }

        public async Task Stop()
        {
            await _discordClient.StopAsync();
            _discordClient?.Dispose();
        }
        
        private Task HandleMessageReceivedAsync(SocketMessage message)
        {
            // _logger.Debug($"Message recived: {message.Channel}-{message.Content}");
            _socketMessageSubject.OnNext(message);
            return Task.CompletedTask;
        }

        private Task HandleClientReadyAsync()
        {
            _logger.Info($"{_discordClient.CurrentUser} is connected to server");
            return Task.CompletedTask;
        }

        private Task HandleLogMessageAsync(LogMessage message)
        {
            _logger.Info($"Log message: {message}");
            return Task.CompletedTask;
        }

        public IObservable<SocketMessage> MessageReceived => _socketMessageSubject.AsObservable();
        
        public async Task SendMessageAsync(ISocketMessageChannel channel, string message)
        {
            _logger.Debug($"Sending message {message} to channel {channel.Name}");
            await channel.SendMessageAsync(message);
        }
    }
}