using FootballResults.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FootballResults.API
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

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            string connectionString = GetConnectionString(); 
            builder.Services.AddDbContext<FootballDataDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
            builder.Services.AddScoped<ICountryRepository, CountryRepository>();
            builder.Services.AddScoped<ITeamRepository, TeamRepository>();
            builder.Services.AddScoped<IMatchRepository, MatchRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            app.Run();
        }
    }
}
