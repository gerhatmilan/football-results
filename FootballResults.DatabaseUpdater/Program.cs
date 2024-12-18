using FootballResults.DataAccess;
using FootballResults.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace FootballResults.DatabaseUpdater
{
    public class Program
    {
        private static HostApplicationBuilder _builder = default!;
        private static IConfiguration _configuration = default!;
        private static IHostEnvironment _environment = default!;

        public static void Main(string[] args)
        {
            IHost host;

            try
            {
                host = InitializeApplication(args);
                host.Run();
            }
            catch (Exception ex)
            {
                if (_environment.IsDevelopment())
                {
                    Console.WriteLine(ex.ToString());
                }
                else
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static IHost InitializeApplication(string[] args)
        {
            _builder = Host.CreateApplicationBuilder(args);
            _environment = _builder.Environment;

            BuildConfiguration();
            CheckConfiguration();
            ConfigureLogging();
            ConfigureServices();

            return _builder.Build();
        }
        
        private static void BuildConfiguration()
        {
            string baseDirectory = AppContext.BaseDirectory;

            _configuration = _builder.Configuration
                .AddJsonFile(Path.Combine(baseDirectory, "appsettings.json"))
                .AddJsonFile(Path.Combine(baseDirectory, $"appsettings.{_environment.EnvironmentName}.json"), optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .CreateLogger();

            _builder.Logging.ClearProviders();
            _builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(Log.Logger);
            });
        }

        private static void CheckConfiguration()
        {
            string configNotFound = string.Empty;
            string key = string.Empty;

            if (_configuration.GetConnectionString("DefaultConnection") == null)
            {
                configNotFound = "Database connection string";
                key = "ConnectionStrings__DefaultConnection";
            }
            else if (_configuration.GetValue<string>(Defaults.FootballApiKeyEncryptionKey) == null)
            {
                configNotFound = "API encryption key";
                key = Defaults.FootballApiKeyEncryptionKey;
            }

            if (!string.IsNullOrEmpty(configNotFound))
            {
                throw new Exception($"{configNotFound} not found. Please provide it in appsettings.json or as an environment variable with key {key}.");
            }
        }

        private static void ConfigureServices()
        {
            _builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            });

            _builder.Services.AddHostedService<UpdaterRunner>();
        }
    }
}