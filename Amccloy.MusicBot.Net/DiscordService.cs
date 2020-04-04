using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using NLog;

namespace Amccloy.MusicBot.Net
{
    /// <summary>
    /// Connects to the discord server and acts as the interface for sending and receiving messages
    /// </summary>
    public class DiscordService : IHostedService, IDiscordService
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private DiscordSocketClient _discordClient;
        private Subject<SocketMessage> _socketMessageSubject = new Subject<SocketMessage>();

        public DiscordService(DiscordSocketClient discordClient)
        {
            _discordClient = discordClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Starting Discord Client Service");
            _discordClient.Log += HandleLogMessageAsync;
            _discordClient.Ready += HandleClientReadyAsync;
            _discordClient.MessageReceived += HandleMessageReceivedAsync;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _discordClient?.Dispose();
            return Task.CompletedTask;
        }
        
        private Task HandleMessageReceivedAsync(SocketMessage message)
        {
            _socketMessageSubject.OnNext(message);
            return Task.CompletedTask;
        }

        private Task HandleClientReadyAsync()
        {
            _logger.Info("Discord client state is: ready");
            return Task.CompletedTask;
        }

        private Task HandleLogMessageAsync(LogMessage message)
        {
            _logger.Info($"Log message: {message}");
            return Task.CompletedTask;
        }

        public IObservable<SocketMessage> MessageReceived => _socketMessageSubject.AsObservable();
        
        public async Task SendMessageAsync(string message, string channel)
        {
            throw new NotImplementedException();
        }
    }

    public interface IDiscordService
    {
        public IObservable<SocketMessage> MessageReceived { get; }
        public Task SendMessageAsync(string message, string channel);
    }
}