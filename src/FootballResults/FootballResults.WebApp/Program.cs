using FootballResults.WebApp.Components;
using FootballResults.WebApp.Services.Football;
using FootballResults.WebApp.Services.Predictions;
using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using FootballResults.WebApp.Hubs;
using FootballResults.WebApp.Services.Chat;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.WebApp.Services.Time;
using Blazored.LocalStorage;
using FootballResults.DataAccess;
using FootballResults.WebApp.Services.Football.Server;
using FootballResults.DataAccess.Repositories.Football;
using FootballResults.API.Models;

namespace FootballResults.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            string databaseApiUrl = builder.Configuration.GetSection("DatabaseApi")["Url"]!;

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddSignalR();
            builder.Services.AddBlazoredLocalStorage();

            // Custom services
            builder.Services.AddScoped<ISignupService, SignupService>();
            builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddTransient<IPredictionGameService, PredictionGameService>();
            builder.Services.AddTransient<IChatService<Message>, GameChatService>();
            builder.Services.AddScoped<IClientTimeService, ClientTimeService>();

            builder.Services.AddScoped<ICountryRepository, CountryRepository>();
            builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
            builder.Services.AddScoped<ITeamRepository, TeamRepository>();
            builder.Services.AddScoped<IMatchRepository, MatchRepository>();

            builder.Services.AddScoped<IMatchService, MatchServiceServer>();
            builder.Services.AddScoped<ILeagueService, LeagueServiceServer>();
            builder.Services.AddScoped<ITeamService, TeamServiceServer>();

            // Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "auth_cookie";
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                    options.Cookie.MaxAge = TimeSpan.FromDays(7);
                    options.AccessDeniedPath = "/access-denied";
                });

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddAuthorization();
            // Database
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                    .LogTo(Console.Write);
            }, ServiceLifetime.Transient);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStatusCodePagesWithRedirects("{0}");

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.MapHub<ChatHub<Message>>("/chathub");

            app.Run();
        }
    }
}
