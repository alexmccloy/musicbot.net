using Amccloy.MusicBot.Net.Configuration;
using Amccloy.MusicBot.Net.Dbo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Amccloy.MusicBot.Net.Model;

public class TriviaContext : DbContext
{
    private readonly PostgresOptions _pgOptions;
    public const string SchemaName = "trivia_questions";
    public DbSet<StandardTriviaQuestionDbo> StandardTriviaQuestions { get; set; }
    public DbSet<MultiChoiceTriviaQuestionDbo> MultiChoiceTriviaQuestions { get; set; }

    public TriviaContext(DbContextOptions<TriviaContext> options, IOptions<PostgresOptions> postgresOptions)
        : base(options)
    {
        _pgOptions = postgresOptions.Value;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql($"User ID={_pgOptions.Username};" +
                                 $"Password={_pgOptions.Password};" +
                                 $"Host={_pgOptions.Hostname};" +
                                 $"Port={_pgOptions.Port};" +
                                 $"Database={_pgOptions.DatabaseName};");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        modelBuilder.Entity<StandardTriviaQuestionDbo>()
                    .HasIndex(question => question.Category);
        modelBuilder.Entity<StandardTriviaQuestionDbo>()
                    .HasIndex(question => question.Source);
        
        modelBuilder.Entity<MultiChoiceTriviaQuestionDbo>()
                    .HasIndex(question => question.Category);
        modelBuilder.Entity<MultiChoiceTriviaQuestionDbo>()
                    .HasIndex(question => question.Source);
    }
}