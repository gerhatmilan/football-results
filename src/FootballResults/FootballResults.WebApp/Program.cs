using Blazored.LocalStorage;
using FootballResults.API.Models;
using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.DataAccess.Repositories.Football;
using FootballResults.Models.Config;
using FootballResults.WebApp.BackgroundServices;
using FootballResults.WebApp.Components;
using FootballResults.WebApp.Hubs;
using FootballResults.WebApp.Services;
using FootballResults.WebApp.Services.Chat;
using FootballResults.WebApp.Services.Football;
using FootballResults.WebApp.Services.Football.Server;
using FootballResults.WebApp.Services.LiveUpdates;
using FootballResults.WebApp.Services.Predictions;
using FootballResults.WebApp.Services.Time;
using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace FootballResults.WebApp
{
    public class Program
    {
        private static WebApplicationBuilder _builder = default!;
        private static IConfiguration _configuration = default!;
        private static IHostEnvironment _environment = default!;

        public static void Main(string[] args)
        {
            BuildApplication(args);
            BuildConfiguration();
            CheckConfiguration();
            ConfigureServices();

            var app = _builder.Build();

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

            app.MapHub<MessageHub<Message>>("/chathub");
            app.MapHub<MessageHub<UpdateMessageType>>("/updatehub");

            app.Run();
        }

        private static void BuildApplication(string[] args)
        {
            _builder = WebApplication.CreateBuilder(args);
            _environment = _builder.Environment;
        }

        private static void BuildConfiguration()
        {
            string _configurationFolder = Path.Combine(_environment.ContentRootPath, "..", "Configuration");

            _configuration = _builder.Configuration
                .AddJsonFile(Path.Combine(_configurationFolder, "SharedSettings.json"))
                .AddJsonFile(Path.Combine(_configurationFolder, $"SharedSettings.{_environment.EnvironmentName}.json"))
                .AddJsonFile(Path.Combine(_environment.ContentRootPath, "appsettings.json"))
                .AddJsonFile(Path.Combine(_environment.ContentRootPath, $"appsettings.{_environment.EnvironmentName}.json"))
                .AddEnvironmentVariables()
                .Build();
        }

        private static void CheckConfiguration()
        {
            string configNotFound = string.Empty;

            if (_configuration.GetConnectionString("DefaultConnection") == null)
            {
                configNotFound = "Database connection string";
            }
            else if (_configuration.GetValue<string>("FootballApiConfig:ApiKey") == null)
            {
                configNotFound = "API key";
            }

            if (!string.IsNullOrEmpty(configNotFound))
            {
                throw new Exception($"{configNotFound} not found. Please provide it in appsettings.json or as an environment variable.");
            }
        }

        private static void ConfigureServices()
        {
            // Add services to the container.
            _builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            _builder.Services.AddSignalR()
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                });
            _builder.Services.AddBlazoredLocalStorage();

            // Custom services
            _builder.Services.AddScoped<ISignupService, SignupService>();
            _builder.Services.AddScoped<ILoginService, LoginService>();
            _builder.Services.AddScoped<IUserService, UserService>();
            _builder.Services.AddScoped<IPredictionGameService, PredictionGameService>();
            _builder.Services.AddTransient<IMessageService<Message>, ChatService>();
            _builder.Services.AddTransient<IMessageService<UpdateMessageType>, UpdateNotificationService>();
            _builder.Services.AddScoped<IClientTimeService, ClientTimeService>();

            _builder.Services.AddScoped<ICountryRepository, CountryRepository>();
            _builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
            _builder.Services.AddScoped<ITeamRepository, TeamRepository>();
            _builder.Services.AddScoped<IMatchRepository, MatchRepository>();

            _builder.Services.AddScoped<IMatchService, MatchServiceServer>();
            _builder.Services.AddScoped<ILeagueService, LeagueServiceServer>();
            _builder.Services.AddScoped<ITeamService, TeamServiceServer>();

            // Background services
            _builder.Services.Configure<ApplicationConfig>(_configuration.GetSection("ApplicationConfig"));
            _builder.Services.Configure<FootballApiConfig>(_configuration.GetSection("FootballApiConfig"));
            _builder.Services.AddHostedService<FootballDataUpdater>();
            _builder.Services.AddHostedService<PredictionGamesUpdater>();
            _builder.Services.AddHostedService<ImageDownloader>();

            // Authentication
            _builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "auth_cookie";
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                    options.Cookie.MaxAge = TimeSpan.FromDays(7);
                    options.AccessDeniedPath = "/access-denied";
                });

            _builder.Services.AddCascadingAuthenticationState();
            _builder.Services.AddAuthorization();

            // Database
            _builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                    .LogTo(Console.Write, minimumLevel: LogLevel.Warning);
            }, ServiceLifetime.Transient);
        }
    }
}
