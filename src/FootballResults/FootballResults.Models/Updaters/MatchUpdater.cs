using FootballResults.DataAccess.Entities;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.Models.Api.FootballApi.Responses;
using FootballResults.Models.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FootballResults.Models.Updaters
{
    [Updater]
    [SupportedModes(UpdaterMode.AllLeaguesAllSeasons, UpdaterMode.AllLeaguesCurrentSeason, UpdaterMode.AllLeaguesSpecificSeason, UpdaterMode.SpecificLeagueAllSeasons, UpdaterMode.SpecificLeagueCurrentSeason, UpdaterMode.CurrentDate, UpdaterMode.SpecificDate)]
    public class MatchUpdater : Updater<FixturesResponse, FixturesResponseItem>
    {
        private readonly ILoggerFactory _loggerFactory;
        private VenueUpdaterForMatch _venueUpdater;

        protected override UpdaterSpecificSettings UpdaterSpecificSettingsForLeagueAndSeason => _apiConfig.DataFetch.MatchesForLeagueAndSeason;
        protected override UpdaterSpecificSettings UpdaterSpecificSettingsForDate => _apiConfig.DataFetch.MatchesForDate;

        public MatchUpdater(IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, ILogger<MatchUpdater> logger)
            : base(serviceScopeFactory, logger)
        {
            _loggerFactory = loggerFactory;
            _venueUpdater = new VenueUpdaterForMatch(serviceScopeFactory, _loggerFactory.CreateLogger<VenueUpdaterForMatch>());
        }

        protected override void ProcessData(IEnumerable<FixturesResponseItem> responseItems)
        {
            ICollection<int> includedLeagues = GetIncludedLeagueIDs();

            responseItems = responseItems.Where(item => item.League.ID.HasValue && includedLeagues.Contains(item.League.ID.Value)); // only matches for included leagues

            _venueUpdater.ProcessData(_dbContext, responseItems); // save venues first
            var mappedMatches = responseItems.Select(MapMatch);

            for (int i = 0; i < responseItems.Count(); i++)
            {
                var matchResponseItem = responseItems.ElementAt(i);
                var responseRecord = mappedMatches.ElementAt(i);

                DataAccess.Entities.Football.LeagueSeason relatedLeagueSeason = _dbContext.LeagueSeasons.FirstOrDefault(leagueSeason => leagueSeason.LeagueID == matchResponseItem.League.ID && leagueSeason.Year == matchResponseItem.League.Season);

                if (responseRecord == null)
                {
                    _logger.LogWarning("Invalid match, skipping...");
                    continue;
                }
                else if (relatedLeagueSeason == null)
                {
                    _logger.LogWarning($"League season does not exist in the database for match {matchResponseItem.Fixture.ID}, skipping...");
                    continue;
                }

                responseRecord.LeagueSeasonID = relatedLeagueSeason.ID;
                responseRecord.LastUpdate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                var existingRecord = _dbContext.Matches.FirstOrDefault(existingRecord => existingRecord.ID.Equals(responseRecord.ID));

                if (existingRecord == null)
                {
                    Add(responseRecord);
                }
                else if (!existingRecord.Equals(responseRecord))
                {
                    Update(existingRecord, responseRecord);
                }
            }

            if (_currentMode == UpdaterMode.AllLeaguesAllSeasons || _currentMode == UpdaterMode.AllLeaguesCurrentSeason || _currentMode == UpdaterMode.AllLeaguesSpecificSeason
                || _currentMode == UpdaterMode.SpecificLeagueCurrentSeason)
            {
                int leagueID = responseItems.FirstOrDefault()?.League.ID ?? -1;
                int year = responseItems.FirstOrDefault()?.League.Season ?? -1;

                DataAccess.Entities.Football.LeagueSeason relatedLeagueSeason = _dbContext.LeagueSeasons.FirstOrDefault(leagueSeason => leagueSeason.LeagueID == leagueID && leagueSeason.Year == year);

                if (relatedLeagueSeason != null)
                {
                    relatedLeagueSeason.MatchesLastUpdate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                }
            }
            else if (_currentMode == UpdaterMode.CurrentDate)
            {
                SystemInformation systemInfo = _dbContext.SystemInformation.Find(1);

                if (systemInfo != null)
                {
                    systemInfo.MatchesLastUpdateForCurrentDay = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                }
            }
        }

        protected void Add(Match match)
        {
            _logger.LogDebug($"Adding new match: {match.ID}");
            _dbContext.Matches.Add(match);
        }

        protected void Update(Match existingRecord, Match responseRecord)
        {
            _logger.LogDebug($"Updating match with ID {existingRecord.ID}:"
                + (existingRecord.ID != responseRecord.ID ? $"\n\tID: {existingRecord.ID} -> {responseRecord.ID}" : "")
                + (existingRecord.LeagueSeasonID != responseRecord.LeagueSeasonID ? $"\n\tLeagueSeasonID: {existingRecord.LeagueSeasonID} -> {responseRecord.LeagueSeasonID}" : "")
                + (existingRecord.VenueID != responseRecord.VenueID ? $"\n\tVenueID: {existingRecord.VenueID} -> {responseRecord.VenueID}" : "")
                + (existingRecord.HomeTeamID != responseRecord.HomeTeamID ? $"\n\tHomeTeamID: {existingRecord.HomeTeamID} -> {responseRecord.HomeTeamID}" : "")
                + (existingRecord.AwayTeamID != responseRecord.AwayTeamID ? $"\n\tAwayTeamID: {existingRecord.AwayTeamID} -> {responseRecord.AwayTeamID}" : "")
                + (existingRecord.Round != responseRecord.Round ? $"\n\tRound: {existingRecord.Round} -> {responseRecord.Round}" : "")
                + (existingRecord.Date != responseRecord.Date ? $"\n\tDate: {existingRecord.Date} -> {responseRecord.Date}" : "")
                + (existingRecord.Status != responseRecord.Status ? $"\n\tStatus: {existingRecord.Status} -> {responseRecord.Status}" : "")
                + (existingRecord.Minute != responseRecord.Minute ? $"\n\tMinute: {existingRecord.Minute} -> {responseRecord.Minute}" : "")
                + (existingRecord.HomeTeamGoals != responseRecord.HomeTeamGoals ? $"\n\tHomeTeamGoals: {existingRecord.HomeTeamGoals} -> {responseRecord.HomeTeamGoals}" : "")
                + (existingRecord.AwayTeamGoals != responseRecord.AwayTeamGoals ? $"\n\tAwayTeamGoals: {existingRecord.AwayTeamGoals} -> {responseRecord.AwayTeamGoals}" : ""));

            existingRecord.CopyFrom(responseRecord);
        }

        public static bool ValidateMatch(FixturesResponseItem responseItem)
        {
            return responseItem.Fixture.ID.HasValue
                && responseItem.League.ID.HasValue
                && responseItem.League.Season.HasValue
                && responseItem.Teams.Home.ID.HasValue
                && responseItem.Teams.Away.ID.HasValue;
        }

        public static Match MapMatch(FixturesResponseItem responseItem)
        {
            if (ValidateMatch(responseItem))
            {
                return new Match
                {
                    ID = responseItem.Fixture.ID!.Value,
                    VenueID = responseItem.Fixture.Venue.ID,
                    HomeTeamID = responseItem.Teams.Home.ID!.Value,
                    AwayTeamID = responseItem.Teams.Away.ID!.Value,
                    Round = responseItem.League.Round,
                    Date = DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeToUtc(responseItem.Fixture.Date!.Value), DateTimeKind.Unspecified),
                    Status = responseItem.Fixture.Status.Short,
                    Minute = responseItem.Fixture.Status.Elapsed,
                    HomeTeamGoals = responseItem.Goals.Home,
                    AwayTeamGoals = responseItem.Goals.Away
                };
            }
            else
            {
                return null;
            }
        }
    }
}