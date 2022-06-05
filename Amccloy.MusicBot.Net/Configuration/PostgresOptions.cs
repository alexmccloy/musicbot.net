namespace Amccloy.MusicBot.Net.Configuration;

public record PostgresOptions
{
    public string Username { get; set; }
    public string Password { get; set; }
    
    public string Hostname { get; set; }
    public int Port { get; set; }
    public string DatabaseName { get; set; }
}