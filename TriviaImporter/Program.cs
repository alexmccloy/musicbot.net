// See https://aka.ms/new-console-template for more information

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
            Environment.Exit(1);
        }
        
        // Prompt for the database password
        Console.Write("Enter postgres password: ");
        string dbPassword = Console.ReadLine() ?? throw new Exception("Password cannot be null");
        
        // Determine which mode we are running in
        switch (Enum.Parse<TriviaSource>(args[0]))
        {
            case TriviaSource.OpenTriviaDatabase:
                var importer = new OpenTriviaDatabaseImporter(dbPassword);
                await importer.Execute();
                break;
            default:
                Console.WriteLine("Error cannot parse database type");
                break;
        }
    }
}