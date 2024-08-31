using FootballResults.DataAccess.Entities.Football;
using FootballResults.Models.Api.FootballApi;
using FootballResults.Models.Api.FootballApi.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FootballResults.DatabaseUpdaters.Updaters
{
    [Updater]
    [SupportedModes(UpdaterMode.AllLeaguesAllSeasons, UpdaterMode.AllLeaguesCurrentSeason, UpdaterMode.AllLeaguesSpecificSeason, UpdaterMode.SpecificLeagueCurrentSeason)]
    public class TopScorerUpdater : Updater<TopscorersResponse, TopscorersResponseItem>
    {
        protected override UpdaterSpecificSettings UpdaterSpecificSettingsForLeagueAndSeason => _apiConfig.DataFetch.TopScorersForLeagueAndSeason;

        public TopScorerUpdater(IServiceScopeFactory serviceScopeFactory, ILogger<TopScorerUpdater> logger, IOptions<FootballApiConfig> apiConfig)
            : base(serviceScopeFactory, logger, apiConfig) { }

        protected override void ProcessData(IEnumerable<TopscorersResponseItem> responseItems)
        {
            int? leagueID = responseItems.FirstOrDefault()?.Statistics?.First()?.League?.ID;
            int? season = responseItems.FirstOrDefault()?.Statistics?.First()?.League?.Season;

            var relatedLeagueSeason = _dbContext.LeagueSeasons
                .Include(ls => ls.League)
                .FirstOrDefault(leagueSeason => leagueSeason.LeagueID == leagueID && leagueSeason.Year == season);

            if (relatedLeagueSeason == null)
            {
                _logger.LogWarning("Invalid league and/or season for top scorers, skipping...");
                return;
            }

            // query only those top scorers that are related for the given league season
            var existingTopScorers = _dbContext.TopScorers
                .Where(ts => ts.LeagueSeasonID == relatedLeagueSeason.ID)
                .ToList();

            var mappedTopScorers = responseItems.Select(MapTopScorer);

            for (int i = 0; i < mappedTopScorers.Count(); i++)
            {
                var mappedTopScorer = mappedTopScorers.ElementAt(i);

                if (mappedTopScorer == null)
                {
                    _logger.LogWarning("Invalid top scorer, skipping...");
                    continue;
                }

                mappedTopScorer.Rank = i + 1; // top scorers come in order
                mappedTopScorer.LeagueSeasonID = relatedLeagueSeason.ID;
                mappedTopScorer.LeagueSeason = relatedLeagueSeason;

                var existingRecord = existingTopScorers.FirstOrDefault(existingRecord => existingRecord.Rank.Equals(mappedTopScorer.Rank));

                if (existingRecord == null)
                {
                    existingTopScorers.Add(mappedTopScorer);
                    Add(mappedTopScorer);
                }
                else if (!existingRecord.Equals(mappedTopScorer))
                    Update(existingRecord, mappedTopScorer);
            }
        }

        protected void Add(TopScorer topScorer)
        {
            _logger.LogDebug($"Adding new topscorer for league {topScorer.League.Name}, season {topScorer.LeagueSeason.Year}: {topScorer.Rank} - {topScorer.PlayerName}");
            _dbContext.TopScorers.Add(topScorer);
        }

        protected void Update(TopScorer existingRecord, TopScorer responseRecord)
        {
            _logger.LogDebug($"Updating topscorer for league {existingRecord.League.Name}, season {existingRecord.LeagueSeason.Year}:"
                + (existingRecord.LeagueSeasonID != responseRecord.LeagueSeasonID ? $"\n\tLeagueSeasonID: {existingRecord.LeagueSeasonID} -> {responseRecord.LeagueSeasonID}" : "")
                + (existingRecord.TeamID != responseRecord.TeamID ? $"\n\tTeamID: {existingRecord.TeamID} -> {responseRecord.TeamID}" : "")
                + (existingRecord.Rank != responseRecord.Rank ? $"\n\tRank: {existingRecord.Rank} -> {responseRecord.Rank}" : "")
                + (existingRecord.PlayerName != responseRecord.PlayerName ? $"\n\tPlayerName: {existingRecord.PlayerName} -> {responseRecord.PlayerName}" : "")
                + (existingRecord.PhotoLink != responseRecord.PhotoLink ? $"\n\tPhotoLink: {existingRecord.PhotoLink} -> {responseRecord.PhotoLink}" : "")
                + (existingRecord.Played != responseRecord.Played ? $"\n\tPlayed: {existingRecord.Played} -> {responseRecord.Played}" : "")
                + (existingRecord.Goals != responseRecord.Goals ? $"\n\tGoals: {existingRecord.Goals} -> {responseRecord.Goals}" : "")
                + (existingRecord.Assists != responseRecord.Assists ? $"\n\tAssists: {existingRecord.Assists} -> {responseRecord.Assists}" : ""));

            existingRecord.CopyFrom(responseRecord);
        }

        public static bool ValidateTopScorer(TopscorersResponseItem responseItem)
        {
            return responseItem.Statistics.First().Team.ID.HasValue
                && !string.IsNullOrEmpty(responseItem.Player.Name)
                && responseItem.Statistics.First().Goals.Total.HasValue;
        }

        public static TopScorer? MapTopScorer(TopscorersResponseItem responseItem)
        {
            if (ValidateTopScorer(responseItem))
            {
                return new TopScorer
                {
                    TeamID = responseItem.Statistics.First().Team.ID!.Value,
                    PlayerName = responseItem.Player.Name,
                    PhotoLink = responseItem.Player.Photo,
                    Played = responseItem.Statistics.First().Games.Appearences,
                    Goals = responseItem.Statistics.First().Goals.Total!.Value,
                    Assists = responseItem.Statistics.First().Goals.Assists,
                };
            }
            else
            {
                return null;
            }
        }
    }
}