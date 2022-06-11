using Amccloy.MusicBot.Net.Commands;
using Amccloy.MusicBot.Net.Configuration;
using Amccloy.MusicBot.Net.Discord;
using Amccloy.MusicBot.Net.Model;
using Amccloy.MusicBot.Net.Trivia;
using Amccloy.MusicBot.Net.Utils;
using Amccloy.MusicBot.Net.Utils.RX;
using Discord;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amccloy.MusicBot.Net
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //Config
            services.Configure<DiscordOptions>(Configuration.GetSection("Discord"));
            services.Configure<PostgresOptions>(Configuration.GetSection("Postgres"));
            
            // Database stuff
            services.AddDbContextFactory<TriviaContext>(optionsBuilder =>
            {
                optionsBuilder.UseNpgsql(builder =>
                              {
                                  builder.UseNetTopologySuite()
                                         .MigrationsHistoryTable("__EFMigrationsHistory", TriviaContext.SchemaName)
                                         .EnableRetryOnFailure(5);
                              })
                              .UseSnakeCaseNamingConvention();
            });
            
            //General stuff
            services.AddSingleton<ISchedulerFactory, DefaultSchedulerFactory>();
            
            // Lavalink stuff
            var lavalinkConfig = Configuration.GetSection("Lavalink");
            services.AddSingleton<IAudioService, LavalinkNode>()
                    .AddSingleton<DiscordSocketClient>()
                    .AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>()
                    .AddSingleton(new LavalinkNodeOptions()
                    {
                        RestUri = lavalinkConfig["RestUri"],
                        WebSocketUri = lavalinkConfig["WebSocketUri"],
                        Password = lavalinkConfig["Password"]
                    }); 
            
            //Discord stuff
            services.AddSingleton<DiscordConnectionManager>()
                    .AddHostedService<CommandProcessingService>()
                    .AddSingleton<IDiscordInterface, DiscordInterface>()
                    .AddSingleton(new DiscordSocketConfig()
                    {
                        MessageCacheSize = 0,
                        AlwaysDownloadUsers = true,

                        GatewayIntents =
                            GatewayIntents.Guilds |
                            GatewayIntents.GuildMembers |
                            GatewayIntents.GuildMessageReactions |
                            GatewayIntents.GuildMessages |
                            GatewayIntents.GuildVoiceStates
                    });

            services.AddDiscordCommand<TestCommand>()
                    .AddDiscordCommand<RenameUserCommand>()
                    .AddDiscordCommand<MusicCommand>()
                    .AddDiscordCommand<TriviaCommand>();

            services.AddTriviaQuestionProvider<SqlTriviaQuestionProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder builder, IWebHostEnvironment env, TriviaContext dbContext)
        {
            // Automatically perform any pending migrations.
            dbContext.Database.Migrate();
            
            if (env.IsDevelopment())
            {
            }
            else
            {
            }
            

        }
    }
}
