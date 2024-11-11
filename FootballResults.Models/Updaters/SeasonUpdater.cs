using FootballResults.DataAccess.Entities;
using FootballResults.DataAccess.Models;
using FootballResults.Models.Api.FootballApi.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FootballResults.Models.Updaters
{
    [Updater]
    [SupportedModes(UpdaterMode.Helper)]
    public class SeasonUpdater : Updater<LeaguesResponse, LeaguesResponseItem>
    {
        protected override EndpointConfig UpdaterSpecificSettings => _endpointConfigs.FirstOrDefault(i => i.Name == Defaults.Leagues);

        public SeasonUpdater(IServiceScopeFactory serviceScopeFactory, ILogger<SeasonUpdater> logger)
            : base(serviceScopeFactory, logger) { }

        protected override void ProcessData(IEnumerable<LeaguesResponseItem> responseItems)
        {
            List<DataAccess.Entities.Football.LeagueSeason> existingLeageSeasons = _dbContext.LeagueSeasons
               .Include(leagueSeason => leagueSeason.League)
               .ToList();

            // filter leagues based on configuration file
            ICollection<int> includedLeagueIDs = LeaguesWithUpdateActive.Select(i => i.ID).ToList();
            var filteredResponseItems = responseItems.Where(responseItem => responseItem.League.ID != null
                && includedLeagueIDs.Contains(responseItem.League.ID.Value));

            foreach (var leagueResponseItem in filteredResponseItems)
            {
                foreach (var seasonResponseItem in leagueResponseItem.Seasons)
                {
                    var mappedLeagueSeason = MapSeason(seasonResponseItem);

                    if (mappedLeagueSeason == null)
                    {
                        _logger.LogWarning($"Invalid league season for {leagueResponseItem.League.Name}, skipping...");
                        continue;
                    }

                    DataAccess.Entities.Football.LeagueSeason existingLeagueSeason = existingLeageSeasons.FirstOrDefault(existingLeagueSeason => existingLeagueSeason?.Year == mappedLeagueSeason.Year
                        && existingLeagueSeason.League?.ID == leagueResponseItem.League.ID);

                    mappedLeagueSeason.LeagueID = leagueResponseItem.League.ID!.Value;
                    DataAccess.Entities.Football.League relatedLeague = _dbContext.Leagues.FirstOrDefault(league => league.ID == leagueResponseItem.League.ID)!;

                    if (existingLeagueSeason == null)
                    {
                        existingLeageSeasons.Add(mappedLeagueSeason);
                        Add(leagueResponseItem.League.Name, mappedLeagueSeason);
                    }
                    else if (!existingLeagueSeason.Equals(mappedLeagueSeason))
                        Update(existingLeagueSeason, mappedLeagueSeason);
                }

                _dbContext.SaveChanges();
            }
        }

        private void Add(string leagueName, DataAccess.Entities.Football.LeagueSeason leagueSeason)
        {
            _logger.LogDebug($"Adding new league season: {leagueName}, {leagueSeason.Year}");
            _dbContext.LeagueSeasons.Add(leagueSeason);
        }

        private void Update(DataAccess.Entities.Football.LeagueSeason existingRecord, DataAccess.Entities.Football.LeagueSeason responseRecord)
        {
            _logger.LogDebug($"Updating league season with ID {existingRecord.ID}:"
                + (existingRecord.Year != responseRecord.Year ? $"\n\tYear: {existingRecord.Year} -> {responseRecord.Year}" : "")
                + (existingRecord.InProgress != responseRecord.InProgress ? $"\n\tInProgress: {existingRecord.InProgress} -> {responseRecord.InProgress}" : ""));

            existingRecord.CopyFrom(responseRecord);
        }

        public static bool ValidateSeason(LeagueSeason responseItem)
        {
            return responseItem.Year.HasValue
                && responseItem.Current.HasValue;
            // && responseItem.Coverage.Standings.GetValueOrDefault(false)
            // && responseItem.Coverage.TopScorers.GetValueOrDefault(false);
        }

        public static DataAccess.Entities.Football.LeagueSeason MapSeason(LeagueSeason responseItem)
        {
            if (ValidateSeason(responseItem))
            {
                return new DataAccess.Entities.Football.LeagueSeason
                {
                    Year = responseItem.Year!.Value,
                    InProgress = responseItem.Current!.Value,
                };
            }
            else
            {
                return null;
            }
        }
    }
}
