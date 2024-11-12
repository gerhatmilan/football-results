using FootballResults.DataAccess.Entities;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Models;
using FootballResults.Models.Api.FootballApi.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FootballResults.Models.Updaters
{
    [Updater]
    [SupportedModes(UpdaterMode.Classic)]
    public class LeagueUpdater : Updater<LeaguesResponse, LeaguesResponseItem>
    {
        private readonly ILoggerFactory _loggerFactory;
        private SeasonUpdater _seasonUpdater;

        protected override EndpointConfig UpdaterSpecificSettings => _endpointConfigs.FirstOrDefault(i => i.Name == Defaults.Leagues);

        public LeagueUpdater(IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, ILogger<LeagueUpdater> logger)
            : base(serviceScopeFactory, logger)
        {
            _loggerFactory = loggerFactory;
            _seasonUpdater = new SeasonUpdater(serviceScopeFactory, _loggerFactory.CreateLogger<SeasonUpdater>());
        }

        protected override void ProcessData(IEnumerable<LeaguesResponseItem> responseItems, UpdaterMode? mode = null)
        {
            var existingLeagues = _dbContext.Leagues
                .Include(league => league.Country)
                .ToList();

            var mappedLeagues = responseItems.Select(MapLeague);

            for (int i = 0; i < responseItems.Count(); i++)
            {
                var leagueResponseItem = responseItems.ElementAt(i);
                var responseRecord = mappedLeagues.ElementAt(i);

                Country relatedCountry = _dbContext.Countries.FirstOrDefault(country => country.Name.Equals(leagueResponseItem.Country.Name));

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
            _seasonUpdater.ProcessData(_dbContext, responseItems, mode);
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

        public static DataAccess.Entities.Football.League MapLeague(LeaguesResponseItem responseItem)
        {
            if (ValidateLeague(responseItem))
            {
                return new DataAccess.Entities.Football.League
                {
                    ID = responseItem.League.ID!.Value,
                    Name = responseItem.League.Name,
                    Type = responseItem.League.Type,
                    LogoLink = responseItem.League.Logo,
                    UpdatesActive = Defaults.DefaultLeagues.Any(defaultLeague => defaultLeague.ID == responseItem.League.ID!.Value)
                };
            }
            else
            {
                return null;
            }
        }
    }
}