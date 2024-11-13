using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Football;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FootballResults.DataAccess.Repositories.Football
{
    public class LeagueRepository : GenericRepository<League>, ILeagueRepository
    {
        public LeagueRepository(AppDbContext dbContext) : base(dbContext) { }

        public override async Task<IEnumerable<League>> GetAllAsync(bool tracking = true)
        {
            return await _dbContext.Leagues
                .Include(l => l.LeagueSeasons)
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .ToListAsync();
        }

        public async Task<League> GetLeagueByName(string leagueName, bool tracking = true)
        {
            return await _dbContext.Leagues
                .Include(l => l.LeagueSeasons)
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .FirstOrDefaultAsync(l => l.Name.ToLower().Equals(leagueName.ToLower()));
        }

        public async Task<IEnumerable<int>> GetSeasonsForLeague(string leagueName, bool tracking = true)
        {
            League league = await _dbContext.Leagues
                .Include(l => l.LeagueSeasons)
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .FirstOrDefaultAsync(l => l.Name.ToLower().Equals(leagueName.ToLower()));

            return league.LeagueSeasons
                .OrderBy(s => s.Year)
                .Select(s => s.Year)
                .ToList();
        }

        public async Task<IEnumerable<Team>> GetTeamsForLeague(string leagueName, int? season, bool tracking = true)
        {
            League league = await _dbContext.Leagues
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .FirstOrDefaultAsync(league => league.Name.ToLower().Equals(leagueName.ToLower()));

            return league != null
                ? await _dbContext.LeagueStandings
                    .Where(s => s.LeagueSeason.LeagueID.Equals(league.ID) && (season.HasValue ? s.LeagueSeason.Year == season : true))
                    .Include(s => s.Team)
                    .Select(s => s.Team)
                    .OrderBy(t => t.Name)
                    .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                    .ToListAsync()
                : Enumerable.Empty<Team>();
        }

        public async Task<IEnumerable<string>> GetRoundsForLeagueAndSeason(string leagueName, int season, bool tracking = true)
        {
            League league = await _dbContext.Leagues
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .FirstOrDefaultAsync(league => league.Name.ToLower().Equals(leagueName.ToLower()));

            var nonDistinctRoundList = league != null
                ? await _dbContext.Matches
                    .Include(m => m.LeagueSeason)
                    .Where(m => m.LeagueSeason.LeagueID == league.ID && m.LeagueSeason.Year == season)
                    .OrderBy(m => m.Date)
                    .ThenBy(m => m.Round)
                    .Select(m => m.Round)
                    .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                    .ToListAsync()
                : Enumerable.Empty<string>();

            var distinctRoundList = nonDistinctRoundList
                .GroupBy(r => r)
                .Select(r => r.First());

            return distinctRoundList;
        }

        public async Task<IEnumerable<Match>> GetMatchesForLeague(string leagueName, int? season, string round, bool tracking = true)
        {
            League league = await _dbContext.Leagues
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .FirstOrDefaultAsync(league => league.Name.ToLower().Equals(leagueName.ToLower()));

            if (league != null)
            {
                var query = _dbContext.Matches
                    .Where(m => m.LeagueSeason.LeagueID.Equals(league.ID)
                        && (season.HasValue ? m.LeagueSeason.Year == season : true)
                        && (round != null ? m.Round.ToLower().Equals(round.ToLower()) : true));

                return await query
                    .OrderBy(m => m.Date)
                    .Select(m => new Match
                    {
                        ID = m.ID,
                        LeagueSeasonID = m.LeagueSeasonID,
                        HomeTeamID = m.HomeTeamID,
                        AwayTeamID = m.AwayTeamID,
                        Date = m.Date,
                        Round = m.Round,
                        Status = m.Status,
                        Minute = m.Minute,
                        HomeTeamGoals = m.HomeTeamGoals,
                        AwayTeamGoals = m.AwayTeamGoals,
                        LastUpdate = m.LastUpdate,
                        HomeTeam = new Team
                        {
                            ID = m.HomeTeam.ID,
                            Name = m.HomeTeam.Name,
                            ShortName = m.HomeTeam.ShortName,
                            LogoLink = m.HomeTeam.LogoLink,
                        },
                        AwayTeam = new Team
                        {
                            ID = m.AwayTeam.ID,
                            Name = m.AwayTeam.Name,
                            ShortName = m.AwayTeam.ShortName,
                            LogoLink = m.AwayTeam.LogoLink,
                        }
                    })
                    .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                    .ToListAsync();
            }
            else
            {
                return Enumerable.Empty<Match>();
            }
        }

        public async Task<IEnumerable<LeagueStanding>> GetStandingsForLeagueAndSeason(string leagueName, int season, bool tracking = true)
        {
            League league = await _dbContext.Leagues
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .FirstOrDefaultAsync(league => league.Name.ToLower().Equals(leagueName.ToLower()));

            return league != null
                ? await _dbContext.LeagueStandings
                    .Include(s => s.LeagueSeason)
                    .Where(s => s.LeagueSeason.LeagueID == league.ID && s.LeagueSeason.Year == season)
                    .Include(s => s.Team)
                    .OrderBy(s => s.Group)
                    .ThenBy(s => s.Rank)
                    .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                    .ToListAsync()
                : Enumerable.Empty<LeagueStanding>();
        }

        public async Task<IEnumerable<TopScorer>> GetTopScorersForLeagueAndSeason(string leagueName, int season, bool tracking = true)
        {
            League league = await _dbContext.Leagues
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .FirstOrDefaultAsync(league => league.Name.ToLower().Equals(leagueName.ToLower()));

            return league != null
                ? await _dbContext.TopScorers
                    .Include(ts => ts.LeagueSeason)
                    .Where(ts => ts.LeagueSeason.LeagueID == league.ID && ts.LeagueSeason.Year == season)
                    .Include(ts => ts.Team)
                    .OrderBy(ts => ts.Rank)
                    .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                    .ToListAsync()
                : Enumerable.Empty<TopScorer>();
        }

        public async Task<IEnumerable<League>> Search(string leagueName, string country, string type, int? currentSeason, bool tracking = true)
        {
            return await _dbContext.Leagues
                .Where(l => (!String.IsNullOrEmpty(leagueName) ? l.Name.ToLower().Contains(leagueName.ToLower()) : true)
                    && (!String.IsNullOrEmpty(country) ? l.Country.Name.ToLower().Contains(country.ToLower()) : true)
                    && (!String.IsNullOrEmpty(type) ? l.Type.ToLower().Equals(type.ToLower()) : true)
                    && (currentSeason.HasValue ? l.LeagueSeasons.FirstOrDefault(leagueSeason => leagueSeason.InProgress).Year == currentSeason : true))
                .Include(l => l.LeagueSeasons)
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .ToListAsync();
        }
    }
}
