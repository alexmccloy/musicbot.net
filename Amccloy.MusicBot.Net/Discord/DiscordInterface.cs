﻿using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Discord.Net;
using Discord.WebSocket;
using NLog;

namespace Amccloy.MusicBot.Net.Discord
{
    /// <summary>
    /// Connects to the discord server and acts as the interface for sending and receiving messages
    /// </summary>
    public class DiscordInterface : IDiscordInterface
    {
        public DiscordSocketClient RawClient { get; }
        public ulong GuildId { get; private set; } = default;

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly Subject<SocketMessage> _socketMessageSubject = new();

        /// <summary>
        /// Please pass me an already started discord client
        /// </summary>
        /// <param name="discordClient">The discord client. It should be already started and logged in
        /// </param>
        public DiscordInterface(DiscordSocketClient discordClient)
        {
            RawClient = discordClient;
            discordClient.MessageReceived += message =>
            {
                _socketMessageSubject.OnNext(message);
                return Task.CompletedTask;
            };

            discordClient.Connected += () =>
            {
                GuildId = RawClient.Guilds.First().Id;
                return Task.CompletedTask;
            };
        }

        public IObservable<SocketMessage> MessageReceived => _socketMessageSubject.AsObservable();
        
        public async Task SendMessageAsync(ISocketMessageChannel channel, string message)
        {
            try
            {
                _logger.Debug($"Sending message {message} to channel {channel.Name}");
                await channel.SendMessageAsync(message);
            }
            catch (HttpException exception)
            {
                _logger.Error(exception, $"Error occured while trying to send message: {exception.Message}");
            }
        }
        
        public SocketUser GetUser(ulong id) => RawClient.GetUser(id);
    }
}