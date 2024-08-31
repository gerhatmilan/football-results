using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.Models.Api.FootballApi;
using FootballResults.Models.Api.FootballApi.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FootballResults.DatabaseUpdaters.Updaters
{
    [Updater]
    [SupportedModes(UpdaterMode.Classic)]
    public class LeagueUpdater : Updater<LeaguesResponse, LeaguesResponseItem>
    {
        private readonly ILoggerFactory _loggerFactory;
        private SeasonUpdater _seasonUpdater;

        protected override UpdaterSpecificSettings UpdaterSpecificSettings => _apiConfig.DataFetch.Leagues;

        public LeagueUpdater(IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, ILogger<LeagueUpdater> logger, IOptions<FootballApiConfig> apiConfig)
            : base(serviceScopeFactory, logger, apiConfig)
        {
            _loggerFactory = loggerFactory;
            _seasonUpdater = new SeasonUpdater(serviceScopeFactory, _loggerFactory.CreateLogger<SeasonUpdater>(), apiConfig);
        }

        protected override void ProcessData(IEnumerable<LeaguesResponseItem> responseItems)
        {
            var existingLeagues = _dbContext.Leagues
                .Include(league => league.Country)
                .ToList();

            // filter leagues based on configuration file
            ICollection<int> includedLeagueIDs = GetIncludedLeagueIDs();
            var filteredResponseItems = responseItems.Where(responseItem => responseItem.League.ID != null
                && includedLeagueIDs.Contains(responseItem.League.ID.Value));

            var mappedLeagues = filteredResponseItems.Select(MapLeague);

            for (int i = 0; i < filteredResponseItems.Count(); i++)
            {
                var leagueResponseItem = filteredResponseItems.ElementAt(i);
                var responseRecord = mappedLeagues.ElementAt(i);

                Country? relatedCountry = _dbContext.Countries.FirstOrDefault(country => country.Name.Equals(leagueResponseItem.Country.Name));

                if (responseRecord == null)
                {
                    _logger.LogWarning("Invalid league, skipping...");
                    continue;
                }
                else if (relatedCountry == null)
                {
                    _logger.LogWarning($"Country does not exist in the database for league {responseRecord.Name}, skipping...");
                    continue;
                }

                responseRecord.CountryID = relatedCountry.ID;
                var existingRecord = existingLeagues.FirstOrDefault(existingRecord => existingRecord.ID.Equals(responseRecord.ID));

                if (existingRecord == null)
                {
                    existingLeagues.Add(responseRecord);
                    Add(responseRecord);
                }
                else if (!existingRecord.Equals(responseRecord))
                    Update(existingRecord, responseRecord);
            }

            _dbContext.SaveChanges();
            _seasonUpdater.ProcessData(_dbContext, filteredResponseItems);
        }

        private void Add(DataAccess.Entities.Football.League league)
        {
            _logger.LogDebug($"Adding new league: {league.Name}");
            _dbContext.Leagues.Add(league);
        }

        private void Update(DataAccess.Entities.Football.League existingRecord, DataAccess.Entities.Football.League responseRecord)
        {
            _logger.LogDebug($"Updating league with ID {existingRecord.ID}:"
                + (existingRecord.ID != responseRecord.ID ? $"\n\tID: {existingRecord.ID} -> {responseRecord.ID}" : "")
                + (existingRecord.CountryID != responseRecord.CountryID ? $"\n\tCountryID: {existingRecord.CountryID} -> {responseRecord.CountryID}" : "")
                + (existingRecord.Name != responseRecord.Name ? $"\n\tName: {existingRecord.Name} -> {responseRecord.Name}" : "")
                + (existingRecord.Type != responseRecord.Type ? $"\n\tType: {existingRecord.Type} -> {responseRecord.Type}" : "")
                + (existingRecord.LogoLink != responseRecord.LogoLink ? $"\n\tLogoLink: {existingRecord.LogoLink} -> {responseRecord.LogoLink}" : ""));

            existingRecord.CopyFrom(responseRecord);
        }

        public static bool ValidateLeague(LeaguesResponseItem responseItem)
        {
            return responseItem.League.ID.HasValue
                && !string.IsNullOrWhiteSpace(responseItem.League.Name)
                && !string.IsNullOrWhiteSpace(responseItem.League.Logo);
        }

        public static DataAccess.Entities.Football.League? MapLeague(LeaguesResponseItem responseItem)
        {
            if (ValidateLeague(responseItem))
            {
                return new DataAccess.Entities.Football.League
                {
                    ID = responseItem.League.ID!.Value,
                    Name = responseItem.League.Name,
                    Type = responseItem.League.Type,
                    LogoLink = responseItem.League.Logo
                };
            }
            else
            {
                return null;
            }
        }
    }
}