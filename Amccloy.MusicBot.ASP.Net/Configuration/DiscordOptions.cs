namespace Amccloy.MusicBot.Asp.Net.Configuration
{
    public record DiscordOptions
    {
        public string BotToken { get; set; }
        public long OwnerId { get; set; }
    }
}