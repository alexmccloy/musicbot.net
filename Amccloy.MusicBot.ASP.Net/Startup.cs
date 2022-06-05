using Amccloy.MusicBot.Asp.Net.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Amccloy.MusicBot.Asp.Net.Discord;
using Amccloy.MusicBot.Asp.Net.Utils.RX;
using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Amccloy.MusicBot.Asp.Net
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
            
            //General stuff
            services.AddSingleton<ISchedulerFactory, DefaultSchedulerFactory>();
            
            //Discord stuff
            services.AddHostedService<DiscordConnectionManager>();
            services.AddTransient<DiscordSocketClient>();
            services.AddSingleton<IDiscordInterface, DiscordInterface>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder builder, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
            }
            else
            {
            }
            

        }
    }
}
