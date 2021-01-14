using Amccloy.MusicBot.Asp.Net.Areas.Identity;
using Amccloy.MusicBot.Asp.Net.Data;
using Amccloy.MusicBot.Asp.Net.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Amccloy.MusicBot.Asp.Net.Discord;
using Amccloy.MusicBot.Asp.Net.Utils.RX;
using BlazorTable;
using DataAccessLibrary;
using DataAccessLibrary.ActivityLogging;
using DataAccessLibrary.DiscordApiToken;
using DataAccessLibrary.Models;
using Discord.WebSocket;

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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddSingleton<WeatherForecastService>();
            
            //General stuff
            services.AddSingleton<ISchedulerFactory, DefaultSchedulerFactory>();
            services.AddBlazorTable();
            
            //Database stuff
            services.AddHostedService<DbInitialiser>();
            services.AddSingleton<IActivityMonitor, ActivityMonitor>();
            services.AddTransient<ISqlDataAccess, SqlDataAccess>();
            services.AddTransient<IPeopleData_SAMPLE, PeopleData_SAMPLE>();
            services.AddTransient<IDiscordApiTokenData, DiscordApiTokenData>();
            services.AddTransient<IActivityData, ActivityData>();
            
            //Discord stuff
            services.AddHostedService<DiscordConnectionManager>();
            services.AddTransient<DiscordSocketClient>();
            services.AddSingleton<IDiscordInterface, DiscordInterface>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
