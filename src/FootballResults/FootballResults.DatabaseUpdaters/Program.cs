using Extensions;
using FootballResults.DataAccess;
using FootballResults.DatabaseUpdaters.UpdaterMenu;
using FootballResults.Models.Api.FootballApi;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FootballResults.DatabaseUpdaters
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
            _configuration = _builder.Configuration;
            _environment = _builder.Environment;

            CheckConfiguration();
            ConfigureLogging();

            _builder.Services.Configure<FootballApiConfig>(_configuration.GetSection("FootballApiConfig"));

            _builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            });            

            _builder.Services.AddHostedService<UpdaterRunner>();

            return _builder.Build();
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
    }
}