using Amccloy.MusicBot.Net.Dbo;
using Dapper;
using Npgsql;

namespace TriviaImporter;

public class OldMusicBotDatabaseImporter
{
    private readonly string _dbPassword;
    private readonly string _connectionString;
    private const string _folderName = "MusicTriviaLists";
    public OldMusicBotDatabaseImporter(string dbPassword)
    {
        _dbPassword = dbPassword;
        _connectionString =
            $"User ID=musicbot;Password={dbPassword};Host=localhost;Port=55432;Database=trivia_questions";
    }

    public async Task Execute()
    {
        using NpgsqlConnection dbCon = new NpgsqlConnection(_connectionString);
        string sqlDelete = @"DELETE FROM trivia_questions.trivia_questions.music_trivia_questions WHERE source=1"; //TODO convert this to a proper enum
        string sqlInsert = @"INSERT INTO trivia_questions.trivia_questions.music_trivia_questions (source, playlist, song_name, song_artist)
                             VALUES (@Source, @Playlist, @SongName, @Artist)";
        await dbCon.OpenAsync();
        
        // Clean up any existing entries
        await dbCon.ExecuteAsync(sqlDelete);
        
        Dictionary<string, int> completedEntries = new();
        Dictionary<string, int> failedEntries = new();
        
        // Get each file 
        foreach (string filename in Directory.EnumerateFiles(_folderName).Where(filename => filename.EndsWith(".txt")))
        {
            Console.WriteLine($"Reading file {filename}");
            completedEntries[filename] = 0;
            failedEntries[filename] = 0;

            var playlistName = Path.GetFileNameWithoutExtension(filename);
            
            foreach (var line in File.ReadLines(filename))
            {
                // Ignore empty lines
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                try
                {
                    // Line is in format "Song Name | Artist Name\n"
                    var split = line.Split('|');
                    var songName = split[0].Trim();
                    var artistName = split[1].Trim();

                    await dbCon.ExecuteAsync(sqlInsert, new
                    {
                        Source = MusicTriviaSource.OldMusicBot,
                        Playlist = playlistName,
                        SongName = songName,
                        Artist = artistName
                    });

                    completedEntries[filename]++;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error on line: {line}");
                    failedEntries[filename]++;
                }
            }
        }
    }
}