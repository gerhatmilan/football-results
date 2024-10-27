using FootballResults.DataAccess;
using FootballResults.Models.Config;
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
            string _configurationFolder = Path.Combine(_environment.ContentRootPath, "..", "Configuration");

            _configuration = _builder.Configuration
                .AddJsonFile(Path.Combine(_configurationFolder, "sharedSettings.json"))
                .AddJsonFile(Path.Combine(_configurationFolder, $"sharedSettings.{_environment.EnvironmentName}.json"))
                .AddJsonFile(Path.Combine(_environment.ContentRootPath, "appsettings.json"))
                .AddJsonFile(Path.Combine(_environment.ContentRootPath, $"appsettings.{_environment.EnvironmentName}.json"))
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
            _builder.Services.Configure<FootballApiConfig>(_configuration.GetSection("FootballApiConfig"));
            _builder.Services.Configure<ApplicationConfig>(_configuration.GetSection("ApplicationConfig"));

            _builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            });

            _builder.Services.AddHostedService<UpdaterRunner>();
        }
    }
}