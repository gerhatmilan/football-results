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
    [SupportedModes(UpdaterMode.AllLeaguesAllSeasons, UpdaterMode.AllLeaguesCurrentSeason, UpdaterMode.AllLeaguesSpecificSeason,
        UpdaterMode.ActiveLeaguesAllSeasons, UpdaterMode.ActiveLeaguesCurrentSeason, UpdaterMode.ActiveLeaguesSpecificSeason,
        UpdaterMode.SpecificLeagueAllSeasons, UpdaterMode.SpecificLeagueCurrentSeason)]
    public class TeamUpdater : Updater<TeamsResponse, TeamsResponseItem>
    {
        private readonly ILoggerFactory _loggerFactory;
        private VenueUpdaterForTeam _venueUpdater;

        protected override EndpointConfig UpdaterSpecificSettingsForLeagueAndSeason => _endpointConfigs.FirstOrDefault(i => i.Name == Defaults.TeamsForLeagueAndSeason);

        public TeamUpdater(IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, ILogger<TeamUpdater> logger)
            : base(serviceScopeFactory, logger)
        {
            _loggerFactory = loggerFactory;
            _venueUpdater = new VenueUpdaterForTeam(serviceScopeFactory, _loggerFactory.CreateLogger<VenueUpdaterForTeam>());
        }

        protected override void ProcessData(IEnumerable<TeamsResponseItem> responseItems, UpdaterMode? mode = null)
        {
            _venueUpdater.ProcessData(_dbContext, responseItems, mode); // save venues first

            var existingTeams = _dbContext.Teams
                .Include(team => team.Country)
                .ToList();

            var mappedTeams = responseItems.Select(MapTeam);

            for (int i = 0; i < responseItems.Count(); i++)
            {
                var teamResponseItem = responseItems.ElementAt(i);
                var responseRecord = mappedTeams.ElementAt(i);

                Country relatedCountry = _dbContext.Countries.FirstOrDefault(country => country.Name.Equals(teamResponseItem.Team.Country));

                if (responseRecord == null)
                {
                    _logger.LogWarning("Invalid team, skipping...");
                    continue;
                }
                else if (relatedCountry == null)
                {
                    _logger.LogWarning($"Country does not exist in the database for team {responseRecord.Name}, skipping...");
                    continue;
                }

                responseRecord.CountryID = relatedCountry.ID;
                var existingRecord = existingTeams.FirstOrDefault(existingRecord => existingRecord.ID.Equals(responseRecord.ID));

                ManipulateResponseRecord(responseRecord, relatedCountry);

                if (existingRecord == null)
                {
                    existingTeams.Add(responseRecord);
                    Add(responseRecord);
                }
                else if (!existingRecord.Equals(responseRecord))
                    Update(existingRecord, responseRecord);
            }
        }

        protected void Add(DataAccess.Entities.Football.Team team)
        {
            _logger.LogDebug($"Adding new team: {team.Name}");
            _dbContext.Teams.Add(team);
        }

        protected void Update(DataAccess.Entities.Football.Team existingRecord, DataAccess.Entities.Football.Team responseRecord)
        {
            _logger.LogDebug($"Updating team with ID {existingRecord.ID}:"
                + (existingRecord.ID != responseRecord.ID ? $"\n\tID: {existingRecord.ID} -> {responseRecord.ID}" : "")
                + (existingRecord.CountryID != responseRecord.CountryID ? $"\n\tCountryID: {existingRecord.CountryID} -> {responseRecord.CountryID}" : "")
                + (existingRecord.VenueID != responseRecord.VenueID ? $"\n\tVenueID: {existingRecord.VenueID} -> {responseRecord.VenueID}" : "")
                + (existingRecord.Name != responseRecord.Name ? $"\n\tName: {existingRecord.Name} -> {responseRecord.Name}" : "")
                + (existingRecord.ShortName != responseRecord.ShortName ? $"\n\tShortName: {existingRecord.ShortName} -> {responseRecord.ShortName}" : "")
                + (existingRecord.LogoLink != responseRecord.LogoLink ? $"\n\tLogoLink: {existingRecord.LogoLink} -> {responseRecord.LogoLink}" : "")
                + (existingRecord.National != responseRecord.National ? $"\n\tNational: {existingRecord.National} -> {responseRecord.National}" : ""));

            existingRecord.CopyFrom(responseRecord);
        }

        public static bool ValidateTeam(TeamsResponseItem responseItem)
        {
            return responseItem.Team.ID.HasValue
                && !string.IsNullOrWhiteSpace(responseItem.Team.Country)
                && !string.IsNullOrWhiteSpace(responseItem.Team.Name)
                && responseItem.Team.National.HasValue;
        }

        public static DataAccess.Entities.Football.Team MapTeam(TeamsResponseItem responseItem)
        {
            if (ValidateTeam(responseItem))
            {
                return new DataAccess.Entities.Football.Team
                {
                    ID = responseItem.Team.ID!.Value,
                    VenueID = responseItem.Venue.ID,
                    Name = responseItem.Team.Name,
                    ShortName = responseItem.Team.Code,
                    LogoLink = responseItem.Team.Logo,
                    National = responseItem.Team.National!.Value
                };
            }
            else
            {
                return null;
            }
        }

        protected void ManipulateResponseRecord(DataAccess.Entities.Football.Team responseRecord, Country relatedCountry)
        {
            if (responseRecord.National)
                ReplaceNationalLogo(responseRecord, relatedCountry);

            if (responseRecord.ShortName == null)
                CreateShortName(responseRecord);
        }

        /// <summary>
        /// Replace the logo of the national team with the flag of its country, but only if the country flag is unique
        /// </summary>
        /// <param name="team"></param>
        /// <param name="relatedCountry"></param>
        protected void ReplaceNationalLogo(DataAccess.Entities.Football.Team team, Country relatedCountry)
        {
            int numberOfCountriesWithSameFlag = _dbContext.Countries.Where(country => country.FlagLink == relatedCountry.FlagLink).Count();

            if (numberOfCountriesWithSameFlag == 1)
            {
                team.LogoLink = relatedCountry.FlagLink;
            }
        }

        protected static void CreateShortName(DataAccess.Entities.Football.Team team)
        {
            if (team.Name.Length >= 3)
            {
                team.ShortName = team.Name.Replace(" ", "").Substring(0, 3);
            }
            else
            {
                team.ShortName = team.Name;
            }
        }
    }
}