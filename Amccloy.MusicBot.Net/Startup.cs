using Amccloy.MusicBot.Net.Configuration;
using Amccloy.MusicBot.Net.Discord;
using Amccloy.MusicBot.Net.Model;
using Amccloy.MusicBot.Net.Utils.RX;
using Discord.WebSocket;
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
            services.AddDbContext<TriviaContext>(optionsBuilder =>
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
            
            //Discord stuff
            services.AddHostedService<DiscordConnectionManager>();
            services.AddTransient<DiscordSocketClient>();
            services.AddSingleton<IDiscordInterface, DiscordInterface>();
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
