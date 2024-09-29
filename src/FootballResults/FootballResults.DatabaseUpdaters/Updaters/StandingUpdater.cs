using FootballResults.DataAccess.Entities.Football;
using FootballResults.Models.Api.FootballApi.Responses;
using FootballResults.Models.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FootballResults.DatabaseUpdaters.Updaters
{
    [Updater]
    [SupportedModes(UpdaterMode.AllLeaguesAllSeasons, UpdaterMode.AllLeaguesCurrentSeason, UpdaterMode.AllLeaguesSpecificSeason, UpdaterMode.SpecificLeagueCurrentSeason)]
    public class StandingUpdater : Updater<StandingsResponse, StandingsResponseItem>
    {
        protected override UpdaterSpecificSettings UpdaterSpecificSettingsForLeagueAndSeason => _apiConfig.DataFetch.StandingsForLeagueAndSeason;

        public StandingUpdater(IServiceScopeFactory serviceScopeFactory, ILogger<StandingUpdater> logger)
            : base(serviceScopeFactory, logger) { }

        protected override void ProcessData(IEnumerable<StandingsResponseItem> responseItems)
        {
            int? leagueID = responseItems.FirstOrDefault()?.League?.ID;
            int? season = responseItems.FirstOrDefault()?.League?.Season;

            var relatedLeagueSeason = _dbContext.LeagueSeasons
                .Include(ls => ls.League)
                .FirstOrDefault(leagueSeason => leagueSeason.LeagueID == leagueID && leagueSeason.Year == season);

            if (relatedLeagueSeason == null)
            {
                _logger.LogWarning("Invalid league and/or season for standings, skipping...");
                return;
            }

            // query only those standings that are related for the given league season
            var existingStandings = _dbContext.LeagueStandings
                .Include(ls => ls.Team)
                .Where(standing => standing.LeagueSeasonID == relatedLeagueSeason.ID)
                .ToList();

            foreach (var responseItem in responseItems)
            {
                // make one big list of list of standings and map each item
                var mappedStandings = responseItem.League.Standings.SelectMany(standingsList => standingsList).Select(MapStanding);

                foreach (var mappedStanding in mappedStandings)
                {
                    if (mappedStanding == null)
                    {
                        _logger.LogWarning("Invalid standing, skipping...");
                        continue;
                    }

                    if (relatedLeagueSeason.League.ID.Equals(2) && relatedLeagueSeason.Year.Equals(2017) && mappedStanding.TeamID.Equals(3772))
                    {
                        // ANOMALY, NOT NEEDED
                        continue;
                    }

                    mappedStanding.LeagueSeasonID = relatedLeagueSeason.ID;
                    mappedStanding.LeagueSeason = relatedLeagueSeason;

                    var existingRecord = existingStandings.FirstOrDefault(existingRecord => existingRecord.TeamID.Equals(mappedStanding.TeamID)
                        && existingRecord.Group.Equals(mappedStanding.Group));

                    if (existingRecord == null)
                    {
                        existingStandings.Add(mappedStanding);
                        Add(mappedStanding);
                    }
                    else if (!existingRecord.Equals(mappedStanding))
                        Update(existingRecord, mappedStanding);
                }
            }

            relatedLeagueSeason.StandingsLastUpdate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        }

        protected void Add(LeagueStanding leagueStanding)
        {
            _logger.LogDebug($"Adding new league standing for league {leagueStanding.League.Name}, season {leagueStanding.LeagueSeason.Year} and team {leagueStanding.TeamID}");
            _dbContext.LeagueStandings.Add(leagueStanding);
        }

        protected void Update(LeagueStanding existingRecord, LeagueStanding responseRecord)
        {
            _logger.LogDebug($"Updating league standing for league {existingRecord.League.Name}, season {existingRecord.LeagueSeason.Year}, team {existingRecord.Team.Name}:"
                + (existingRecord.LeagueSeasonID != responseRecord.LeagueSeasonID ? $"\n\tLeagueSeasonID: {existingRecord.LeagueSeasonID} -> {responseRecord.LeagueSeasonID}" : "")
                + (existingRecord.TeamID != responseRecord.TeamID ? $"\n\tTeamID: {existingRecord.TeamID} -> {responseRecord.TeamID}" : "")
                + (existingRecord.Rank != responseRecord.Rank ? $"\n\tRank: {existingRecord.Rank} -> {responseRecord.Rank}" : "")
                + (existingRecord.Group != responseRecord.Group ? $"\n\tGroup: {existingRecord.Group} -> {responseRecord.Group}" : "")
                + (existingRecord.Points != responseRecord.Points ? $"\n\tPoints: {existingRecord.Points} -> {responseRecord.Points}" : "")
                + (existingRecord.Played != responseRecord.Played ? $"\n\tPlayed: {existingRecord.Played} -> {responseRecord.Played}" : "")
                + (existingRecord.Wins != responseRecord.Wins ? $"\n\tWins: {existingRecord.Wins} -> {responseRecord.Wins}" : "")
                + (existingRecord.Draws != responseRecord.Draws ? $"\n\tDraws: {existingRecord.Draws} -> {responseRecord.Draws}" : "")
                + (existingRecord.Losses != responseRecord.Losses ? $"\n\tLosses: {existingRecord.Losses} -> {responseRecord.Losses}" : "")
                + (existingRecord.Scored != responseRecord.Scored ? $"\n\tScored: {existingRecord.Scored} -> {responseRecord.Scored}" : "")
                + (existingRecord.Conceded != responseRecord.Conceded ? $"\n\tConceded: {existingRecord.Conceded} -> {responseRecord.Conceded}" : ""));

            existingRecord.CopyFrom(responseRecord);
        }

        public static bool ValidateStanding(Standing responseItem)
        {
            return responseItem.Team.ID.HasValue
                && responseItem.Rank.HasValue
                && responseItem.Points.HasValue
                && responseItem.All.Played.HasValue
                && responseItem.All.Win.HasValue
                && responseItem.All.Draw.HasValue
                && responseItem.All.Lose.HasValue
                && responseItem.All.Goals.For.HasValue
                && responseItem.All.Goals.Against.HasValue;
        }

        public static LeagueStanding? MapStanding(Standing responseItem)
        {
            if (ValidateStanding(responseItem))
            {
                return new LeagueStanding
                {
                    TeamID = responseItem.Team.ID!.Value,
                    Rank = responseItem.Rank!.Value,
                    Group = responseItem.Group,
                    Points = responseItem.Points!.Value,
                    Played = responseItem.All.Played!.Value,
                    Wins = responseItem.All.Win!.Value,
                    Draws = responseItem.All.Draw!.Value,
                    Losses = responseItem.All.Lose!.Value,
                    Scored = responseItem.All.Goals.For!.Value,
                    Conceded = responseItem.All.Goals.Against!.Value
                };
            }
            else
            {
                return null;
            }
        }
    }
}