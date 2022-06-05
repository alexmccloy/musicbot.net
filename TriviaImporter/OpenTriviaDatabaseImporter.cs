using System.Net;
using Npgsql;

namespace TriviaImporter;

public class OpenTriviaDatabaseImporter
{
    private readonly string _dbPassword;
    private readonly string _connectionString;
    private const string _folderName = "opentriviadb";

    public OpenTriviaDatabaseImporter(string dbPassword)
    {
        _dbPassword = dbPassword;
        _connectionString = $"User ID=musicbot;Password={dbPassword};Host=localhost;Port=55432;Database=trivia;Pooling=true;Min Pool Size=0;Max Pool Size=100;Connection Lifetime=0"
    }

    public async Task Execute()
    {
        // Delete the existing git repo if it exists
        if (Directory.Exists(_folderName))
        {
            Directory.Delete(_folderName, recursive:true);
        }
        Directory.CreateDirectory(_folderName);
        
        // Download the files from git
        Console.WriteLine("Downloading zip from git");
        string zipPath = Path.Join(_folderName, "opentriviadb.zip");
        //TODO convert this to HttpClient
        using var client = new WebClient();
        client.Headers.Add("user-agent", "Anything");
        client.DownloadFile(
            address: "https://github.com/el-cms/Open-trivia-database/archive/refs/heads/master.zip",
            fileName: zipPath);
        
        System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, Path.Join(_folderName, "output"));

        // Remove all OpenTriviaDatabase files from the database
        using NpgsqlConnection dbCon = new NpgsqlConnection(_connectionString);
        var sql = "DELETE FROM trivia_questions WHERE source=1";
        var deleteCommand = new NpgsqlCommand(sql, dbCon);
        await dbCon.OpenAsync();
        await deleteCommand.ExecuteNonQueryAsync();

        // Import the new files into the database

    }
}