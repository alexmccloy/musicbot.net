// See https://aka.ms/new-console-template for more information

using Amccloy.MusicBot.Net.Dbo;

namespace TriviaImporter;

public class Program
{

    public static async Task Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine($"Please specify which type of trivia database you will be importing. Possible sources are:");
            foreach (var source in Enum.GetValues<TriviaSource>())
            {
                Console.WriteLine(source.ToString());
            }
            foreach (var source in Enum.GetValues<MusicTriviaSource>())
            {
                Console.WriteLine(source.ToString());
            }
            Environment.Exit(1);
        }
        
        // Prompt for the database password
        Console.Write("Enter postgres password: ");
        string dbPassword = Console.ReadLine() ?? throw new Exception("Password cannot be null");
        
        // Determine which mode we are running in

        if (Enum.TryParse<TriviaSource>(args[0], out var triviaSource))
        {
            switch (triviaSource)
            {
                case TriviaSource.OpenTriviaDatabase:
                    var importer = new OpenTriviaDatabaseImporter(dbPassword);
                    await importer.Execute();
                    break;
            }
        }
        else if (Enum.TryParse<MusicTriviaSource>(args[0], out var musicTriviaSource))
        {
            switch (musicTriviaSource)
            {
                case MusicTriviaSource.OldMusicBot:
                    var importer = new OldMusicBotDatabaseImporter(dbPassword);
                    await importer.Execute();
                    break;
            }
        }
        else
        {
            Console.WriteLine("Cannot find matching importer");
        }
    }
}