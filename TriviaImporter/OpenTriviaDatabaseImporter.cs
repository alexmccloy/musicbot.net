using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Amccloy.MusicBot.Net.Dbo;
using Dapper;
using Npgsql;
using Npgsql.Logging;

namespace TriviaImporter;

public class OpenTriviaDatabaseImporter
{
    private readonly string _dbPassword;
    private readonly string _connectionString;
    private const string _folderName = "opentriviadb";

    public OpenTriviaDatabaseImporter(string dbPassword)
    {
        _dbPassword = dbPassword;
        _connectionString =
            $"User ID=musicbot;Password={dbPassword};Host=localhost;Port=55432;Database=trivia_questions";
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
        
        System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, _folderName);

        using NpgsqlConnection dbCon = new NpgsqlConnection(_connectionString);
        string sqlDelete = @"DELETE FROM trivia_questions.trivia_questions.standard_trivia_questions WHERE source=1;
                             DELETE FROM trivia_questions.trivia_questions.multi_choice_trivia_questions WHERE source=1"; //TODO this source could change since its an enum?
        string sqlInsert = @"INSERT INTO trivia_questions.trivia_questions.standard_trivia_questions (category, question, answer, source)
                             VALUES (@Category, @Question, @Answer, @Source)";
        await dbCon.OpenAsync();
        
        // Remove all OpenTriviaDatabase files from the database
        await dbCon.ExecuteAsync(sqlDelete);

        Dictionary<string, int> completedEntries = new();
        Dictionary<string, int> failedEntries = new();
        int multiChoiceCount = 0;

        // Import the new files into the database
        foreach (string filename in Directory.EnumerateFiles(Path.Join(_folderName, "Open-trivia-database-master/en/todo"))
                                       .Where(filename => filename.EndsWith(".json")))
        {
            Console.WriteLine($"Reading file {filename}");
            completedEntries[filename] = 0;
            failedEntries[filename] = 0;
            
            // Read files line by line instead of converting into json since some of the json is fucked by quotes
            foreach (string line in System.IO.File.ReadLines(filename))
            {
                if (line.Trim() == "[" || line.Trim() == "]") continue;
                
                //Remove trailing commas
                string cleanedLine = line.Trim();
                if (cleanedLine.EndsWith(',')) cleanedLine = cleanedLine.Substring(0, cleanedLine.Length - 1);
                
                try
                {
                    JsonObject? json = JsonNode.Parse(cleanedLine)?.AsObject();
                    if (json != null)
                    {
                        // work out if its multi choice or not by checking how many answers there are
                        if (json["answers"]!.AsArray().Count > 1)
                        {
                            // multi choice
                            multiChoiceCount++;
                        }
                        else
                        {
                            // Standard choice
                            var question = new StandardTriviaQuestionDbo
                            {
                                Source = TriviaSource.OpenTriviaDatabase,
                                Category = ParseCategory(json["category_id"]?.GetValue<string>() ??
                                                         throw new InvalidOperationException($"Invalid category for line {line}")),
                                Question = json["question"]?.GetValue<string>() ??
                                           throw new InvalidOperationException($"Invalid question for line {line}"),
                                Answer = json["answers"]?[0]?.GetValue<string>() ??
                                         throw new InvalidOperationException($"Invalid answer for line {line}")
                            };
                             
                            // Insert into the db
                            await dbCon.ExecuteAsync(sqlInsert, question);

                            // Metrics time
                            completedEntries[filename]++;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(cleanedLine);
                    Console.WriteLine("Error: " + e.Message);
                    
                    failedEntries[filename]++;
                }
            }
        }

        foreach (var filename in completedEntries.Keys)
        {
            Console.WriteLine($"Filename: {filename} - Completed {completedEntries[filename]} | Failed {failedEntries[filename]}");
        }
        Console.WriteLine($"Found {multiChoiceCount} multi choice questions that were ignored");
    }

    private TriviaCategory ParseCategory(string category)
    {
        return category.ToUpper() switch
        {
            "ART_AND_LITERATURE" => TriviaCategory.ArtsAndLiterature,
            "ENTERTAINMENT" => TriviaCategory.Entertainment,
            "FOOD_AND_DRINKS" => TriviaCategory.FoodAndDrink,
            "GEOGRAPHY" => TriviaCategory.Geography,
            "HISTORY" => TriviaCategory.History,
            "LANGUAGE" => TriviaCategory.Language,
            "MATHEMATICS" => TriviaCategory.Mathematics,
            "MUSIC" => TriviaCategory.Music,
            "PEOPLE_AND_PLACES" => TriviaCategory.PeopleAndPlaces,
            "RELIGION_AND_MYTHOLOGY" => TriviaCategory.ReligionAndMythology,
            "SCIENCE_AND_NATURE" => TriviaCategory.ScienceAndNature,
            "SPORT_AND_LEISURE" => TriviaCategory.SportAndLeisure,
            "TECH_AND_VIDEO_GAMES" => TriviaCategory.TechAndVideoGames,
            "TOYS_AND_GAMES" => TriviaCategory.ToysAndGames,
            "UNCATEGORIZED" => TriviaCategory.Uncategorized,
            _ => throw new InvalidDataException($"Unknown trivia category {category}")
        };
    }
}