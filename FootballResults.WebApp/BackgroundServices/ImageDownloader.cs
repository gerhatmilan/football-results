using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.Models.Files;
using FootballResults.WebApp.Services.Application;
using FootballResults.WebApp.Services.Predictions;

namespace FootballResults.WebApp.BackgroundServices
{
    public class ImageDownloader : BackgroundService
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ILogger<ImageDownloader> _logger;
        protected readonly ApplicationConfig _applicationConfig;

        private const string WWWROOT = "wwwroot";

        public ImageDownloader(IServiceProvider serviceProvider, ILogger<ImageDownloader> logger)
        {
            _serviceProvider = serviceProvider;

            using (var scope = _serviceProvider.CreateScope())
            {
                IApplicationService applicationService = scope.ServiceProvider.GetRequiredService<IApplicationService>();
                _applicationConfig = applicationService.GetApplicationConfigAsync().Result;
            }

            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (PeriodicTimer timer = new PeriodicTimer(_applicationConfig.ImageDownloadWorkerFrequency))
            {
                do
                {
                    _logger.LogInformation($"Image downloader worker started");

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        using (var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                        {
                            try
                            {
                                await dbContext.Entry(_applicationConfig).ReloadAsync();

                                await DownloadCountryFlagsAsync(dbContext);
                                await DownloadLeagueLogosAsync(dbContext);
                                await DownloadTeamLogosAsync(dbContext);
                                await DownloadPlayerPhotosAsync(dbContext);
                                await DownloadTopScorerPhotosAsync(dbContext);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "An error occurred while downloading image");
                            }
                        }
                    }

                    _logger.LogInformation($"Image downloader worker finished");
                }
                while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken));
            }
        }

        protected bool ShouldUpdate(DateTime? lastUpdate, TimeSpan maximumPassedTime)
        {
            return !lastUpdate.HasValue || lastUpdate.Value.Add(maximumPassedTime) < DateTime.UtcNow;
        }

        protected async Task DownloadCountryFlagsAsync(AppDbContext dbContext)
        {
            IEnumerable<Country> countries = dbContext.Countries.AsEnumerable();

            SystemInformation? systemInfo = await dbContext.SystemInformation.FindAsync(1);
            DateTime? lastUpdate = systemInfo?.CountryFlagsLastDownload;

            bool shouldUpdateBasedOnLastDownload = ShouldUpdate(lastUpdate, _applicationConfig.ImageDownloadFrequency);

            // if there is no last download date, or the last download date is older than the specified, download (or update) all files
            if (countries.Any() && systemInfo != null && shouldUpdateBasedOnLastDownload)
            {
                List<int> failed = new List<int>();

                foreach (Country country in countries)
                {
                    if (country.FlagLink != null)
                    {
                        try
                        {
                            string combinedPath = Path.Combine(_applicationConfig.CountriesDirectory, Path.GetFileName(country.FlagLink));
                            string wwwrootPath = Path.Combine(WWWROOT, combinedPath);
                            _logger.LogInformation($"Downloading country flag for {country.Name} to {wwwrootPath}...");
                            await FileManager.DownloadFileAsync(country.FlagLink, wwwrootPath);
                            country.FlagPath = combinedPath;
                            _logger.LogInformation($"Download completed successfully for {country.Name}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Failed to download country flag for {country.Name}: {ex.Message}");
                            failed.Add(country.ID);
                        }
                    }
                }

                _logger.LogInformation($"Country flags downloading completed");

                if (failed.Count > 0)
                {
                    _logger.LogWarning($"Some country flags failed to download. Failed IDs: {string.Join(",", failed)}");
                }

                systemInfo.CountryFlagsLastDownload = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                await dbContext.SaveChangesAsync();
            }

            bool anyPathMissing = countries.Any(i => i.FlagLink != null && i.FlagPath == null);

            // download only the missing files
            if (anyPathMissing)
            {
                foreach (Country country in countries.Where(i => i.FlagLink != null && i.FlagPath == null))
                {
                    try
                    {
                        string combinedPath = Path.Combine(_applicationConfig.CountriesDirectory, Path.GetFileName(country.FlagLink));
                        string wwwrootPath = Path.Combine(WWWROOT, combinedPath);
                        _logger.LogInformation($"Downloading country flag for {country.Name} to {wwwrootPath}...");
                        await FileManager.DownloadFileAsync(country.FlagLink, wwwrootPath);
                        country.FlagPath = combinedPath;
                        _logger.LogInformation($"Download completed successfully for {country.Name}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to download country flag for {country.Name}: {ex.Message}");
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }

        protected async Task DownloadLeagueLogosAsync(AppDbContext dbContext)
        {
            IEnumerable<League> leagues = dbContext.Leagues.AsEnumerable();

            SystemInformation? systemInfo = await dbContext.SystemInformation.FindAsync(1);
            DateTime? lastUpdate = systemInfo?.LeagueLogosLastDownload;

            bool shouldUpdateBasedOnLastDownload = ShouldUpdate(lastUpdate, _applicationConfig.ImageDownloadFrequency);

            if (leagues.Any() && systemInfo != null && shouldUpdateBasedOnLastDownload)
            {
                List<int> failed = new List<int>();

                foreach (League league in leagues)
                {
                    if (league.LogoLink != null)
                    {
                        try
                        {
                            string combinedPath = Path.Combine(_applicationConfig.LeaguesDirectory, Path.GetFileName(league.LogoLink));
                            string wwwrootPath = Path.Combine(WWWROOT, combinedPath);
                            _logger.LogInformation($"Downloading league logo for {league.Name} to {wwwrootPath}...");
                            await FileManager.DownloadFileAsync(league.LogoLink, wwwrootPath);
                            league.LogoPath = combinedPath;
                            _logger.LogInformation($"Download completed successfully for {league.Name}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Failed to download league logo for {league.Name}: {ex.Message}");
                            failed.Add(league.ID);
                        }
                    }
                }

                _logger.LogInformation($"League logos downloading completed");

                if (failed.Count > 0)
                {
                    _logger.LogWarning($"Some league logos failed to download. Failed IDs: {string.Join(",", failed)}");
                }

                systemInfo.LeagueLogosLastDownload = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                await dbContext.SaveChangesAsync();
            }

            bool anyPathMissing = leagues.Any(i => i.LogoLink != null && i.LogoPath == null);

            if (anyPathMissing)
            {
                foreach (League league in leagues.Where(i => i.LogoLink != null && i.LogoPath == null))
                {
                    try
                    {
                        string combinedPath = Path.Combine(_applicationConfig.LeaguesDirectory, Path.GetFileName(league.LogoLink));
                        string wwwrootPath = Path.Combine(WWWROOT, combinedPath);
                        _logger.LogInformation($"Downloading league logo for {league.Name} to {wwwrootPath}...");
                        await FileManager.DownloadFileAsync(league.LogoLink, wwwrootPath);
                        league.LogoPath = combinedPath;
                        _logger.LogInformation($"Download completed successfully for {league.Name}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to download league logo for {league.Name}: {ex.Message}");
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }

        protected async Task DownloadTeamLogosAsync(AppDbContext dbContext)
        {
            IEnumerable<Team> teams = dbContext.Teams.AsEnumerable();

            SystemInformation? systemInfo = await dbContext.SystemInformation.FindAsync(1);
            DateTime? lastUpdate = systemInfo?.TeamLogosLastDownload;

            bool shouldUpdateBasedOnLastDownload = ShouldUpdate(lastUpdate, _applicationConfig.ImageDownloadFrequency);

            if (teams.Any() && systemInfo != null && shouldUpdateBasedOnLastDownload)
            {
                List<int> failed = new List<int>();

                foreach (Team team in teams)
                {
                    if (team.LogoLink != null)
                    {
                        try
                        {
                            string combinedPath = Path.Combine(_applicationConfig.TeamsDirectory, Path.GetFileName(team.LogoLink));
                            string wwwrootPath = Path.Combine(WWWROOT, combinedPath);
                            _logger.LogInformation($"Downloading team logo for {team.Name} to {wwwrootPath}...");
                            await FileManager.DownloadFileAsync(team.LogoLink, wwwrootPath);
                            team.LogoPath = combinedPath;
                            _logger.LogInformation($"Download completed successfully for {team.Name}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Failed to download team logo for {team.Name}: {ex.Message}");
                            failed.Add(team.ID);
                        }
                    }
                }

                if (failed.Count > 0)
                {
                    _logger.LogWarning($"Some team logos failed to download. Failed IDs: {string.Join(",", failed)}");
                }

                systemInfo.TeamLogosLastDownload = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                await dbContext.SaveChangesAsync();
            }

            bool anyPathMissing = teams.Any(i => i.LogoLink != null && i.LogoPath == null);

            if (anyPathMissing)
            {
                foreach (Team team in teams.Where(i => i.LogoLink != null && i.LogoPath == null))
                {
                    try
                    {
                        string combinedPath = Path.Combine(_applicationConfig.TeamsDirectory, Path.GetFileName(team.LogoLink));
                        string wwwrootPath = Path.Combine(WWWROOT, combinedPath);
                        _logger.LogInformation($"Downloading team logo for {team.Name} to {wwwrootPath}...");
                        await FileManager.DownloadFileAsync(team.LogoLink, wwwrootPath);
                        team.LogoPath = combinedPath;
                        _logger.LogInformation($"Download completed successfully for {team.Name}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to download team logo for {team.Name}: {ex.Message}");
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }

        protected async Task DownloadPlayerPhotosAsync(AppDbContext dbContext)
        {
            IEnumerable<Player> players = dbContext.Players.AsEnumerable();

            SystemInformation? systemInfo = await dbContext.SystemInformation.FindAsync(1);
            DateTime? lastUpdate = systemInfo?.PlayerPhotosLastDownload;

            bool shouldUpdateBasedOnLastDownload = ShouldUpdate(lastUpdate, _applicationConfig.ImageDownloadFrequency);

            if (players.Any() && systemInfo != null && shouldUpdateBasedOnLastDownload)
            {
                List<int> failed = new List<int>();

                foreach (Player player in players)
                {
                    if (player.PhotoLink != null)
                    {
                        try
                        {
                            string combinedPath = Path.Combine(_applicationConfig.PlayersDirectory, Path.GetFileName(player.PhotoLink));
                            string wwwrootPath = Path.Combine(WWWROOT, combinedPath);
                            _logger.LogInformation($"Downloading player photo for {player.Name} to {wwwrootPath}...");
                            await FileManager.DownloadFileAsync(player.PhotoLink, wwwrootPath);
                            player.PhotoPath = combinedPath;
                            _logger.LogInformation($"Download completed successfully for {player.Name}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Failed to download player photo for {player.Name}: {ex.Message}");
                            failed.Add(player.ID);
                        }
                    }
                }

                if (failed.Count > 0)
                {
                    _logger.LogWarning($"Some player photos failed to download. Failed IDs: {string.Join(",", failed)}");
                }

                systemInfo.PlayerPhotosLastDownload = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                await dbContext.SaveChangesAsync();
            }

            bool anyPathMissing = players.Any(i => i.PhotoLink != null && i.PhotoPath == null);

            if (anyPathMissing)
            {
                foreach (Player player in players.Where(i => i.PhotoLink != null && i.PhotoPath == null))
                {
                    try
                    {
                        string combinedPath = Path.Combine(_applicationConfig.PlayersDirectory, Path.GetFileName(player.PhotoLink));
                        string wwwrootPath = Path.Combine(WWWROOT, combinedPath);
                        _logger.LogInformation($"Downloading player photo for {player.Name} to {wwwrootPath}...");
                        await FileManager.DownloadFileAsync(player.PhotoLink, wwwrootPath);
                        player.PhotoPath = combinedPath;
                        _logger.LogInformation($"Download completed successfully for {player.Name}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to download player photo for {player.Name}: {ex.Message}");
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }

        protected async Task DownloadTopScorerPhotosAsync(AppDbContext dbContext)
        {
            IEnumerable<TopScorer> topscorers = dbContext.TopScorers.AsEnumerable();

            SystemInformation? systemInfo = await dbContext.SystemInformation.FindAsync(1);
            DateTime? lastUpdate = systemInfo?.TopScorerPhotosLastDownload;

            bool shouldUpdateBasedOnLastDownload = ShouldUpdate(lastUpdate, _applicationConfig.ImageDownloadFrequency);

            if (topscorers.Any() && systemInfo != null && shouldUpdateBasedOnLastDownload)
            {
                List<int> failed = new List<int>();

                foreach (TopScorer topscorer in topscorers)
                {
                    if (topscorer.PhotoLink != null)
                    {
                        try
                        {
                            string combinedPath = Path.Combine(_applicationConfig.PlayersDirectory, Path.GetFileName(topscorer.PhotoLink));
                            string wwwrootPath = Path.Combine(WWWROOT, combinedPath);
                            _logger.LogInformation($"Downloading player photo for {topscorer.PlayerName} to {wwwrootPath}...");
                            await FileManager.DownloadFileAsync(topscorer.PhotoLink, wwwrootPath);
                            topscorer.PhotoPath = combinedPath;
                            _logger.LogInformation($"Download completed successfully for {topscorer.PlayerName}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Failed to download player photo for {topscorer.PlayerName}: {ex.Message}");
                            failed.Add(topscorer.ID);
                        }
                    }
                }

                if (failed.Count > 0)
                {
                    _logger.LogWarning($"Some player photos failed to download certain top scorers. Failed topscorer IDs: {string.Join(",", failed)}");
                }

                systemInfo.TopScorerPhotosLastDownload = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                await dbContext.SaveChangesAsync();
            }

            bool anyPathMissing = topscorers.Any(i => i.PhotoLink != null && i.PhotoPath == null);

            if (anyPathMissing)
            {
                foreach (TopScorer topscorer in topscorers.Where(i => i.PhotoLink != null && i.PhotoPath == null))
                {
                    try
                    {
                        string combinedPath = Path.Combine(_applicationConfig.PlayersDirectory, Path.GetFileName(topscorer.PhotoLink));
                        string wwwrootPath = Path.Combine(WWWROOT, combinedPath);
                        _logger.LogInformation($"Downloading player photo for {topscorer.PlayerName} to {wwwrootPath}...");
                        await FileManager.DownloadFileAsync(topscorer.PhotoLink, wwwrootPath);
                        topscorer.PhotoPath = combinedPath;
                        _logger.LogInformation($"Download completed successfully for {topscorer.PlayerName}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to download player photo for {topscorer.PlayerName}: {ex.Message}");
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
