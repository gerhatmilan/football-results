using FootballResults.WebApp.Components;
using FootballResults.WebApp.Database;
using FootballResults.WebApp.Services.Football;
using FootballResults.WebApp.Services.Predictions;
using FootballResults.WebApp.Services.Users;
using FootballResults.WebApp.Services.Files;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace FootballResults.WebApp
{
    public class Program
    {
        private static string GetConnectionString()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();

            string host = configuration.GetConnectionString("Host")!;
            string port = configuration.GetConnectionString("Port")!;
            string database = configuration.GetConnectionString("Database")!;

            string usernameEnvVar = configuration.GetConnectionString("UsernameEnvVar")!;
            string passwordEnvVar = configuration.GetConnectionString("PasswordEnvVar")!;

            string username = Environment.GetEnvironmentVariable(usernameEnvVar, EnvironmentVariableTarget.Machine)!;
            string password = Environment.GetEnvironmentVariable(passwordEnvVar, EnvironmentVariableTarget.Machine)!;

            return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Custom services
            builder.Services.AddScoped<ISignupService, SignupService>();
            builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IPredictionGameService, PredictionGameService>();
            builder.Services.AddScoped<IFileUploadService, FileUploadService>();

            // HttpClient services
            builder.Services.AddHttpClient<IMatchService, MatchService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:10001");
            });

            builder.Services.AddHttpClient<ILeagueService, LeagueService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:10001");
            });

            builder.Services.AddHttpClient<ITeamService, TeamService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:10001");
            });

            builder.Services.AddHttpClient<IUserService, UserService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:10001");
            });

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
                options.UseNpgsql(GetConnectionString());
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
