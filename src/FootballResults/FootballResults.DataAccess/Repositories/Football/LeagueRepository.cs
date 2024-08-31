using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Football;
using Microsoft.EntityFrameworkCore;

namespace FootballResults.DataAccess.Repositories.Football
{
    public class LeagueRepository : GenericRepository<League>, ILeagueRepository
    {
        public LeagueRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<League> GetLeagueByName(string leagueName)
        {
            return await _dbContext.Leagues
                .FirstOrDefaultAsync(l => l.Name.ToLower().Equals(leagueName.ToLower()));
        }

        public async Task<IEnumerable<int>> GetSeasonsForLeague(string leagueName)
        {
            return await _dbContext.Leagues
                .Where(l => l.Name.ToLower().Equals(leagueName.ToLower()))
                .Include(l => l.LeagueSeasons)
                .SelectMany(l => l.LeagueSeasons)
                .OrderBy(s => s.Year)
                .Select(s => s.Year)
                .ToListAsync();
        }

        public async Task<IEnumerable<Team>> GetTeamsForLeague(string leagueName, int? season)
        {
            var query = _dbContext.Matches
                .Where(m => m.League.Name.ToLower().Equals(leagueName.ToLower()));

            if (season != null)
                query = query.Where(m => m.LeagueSeason.Year == season);

            return await query
                .Select(m => m.HomeTeam)
                .Union(
                    query.Select(m => m.AwayTeam)
                )
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetRoundsForLeagueAndSeason(string leagueName, int season)
        {

            var nonDistinctRoundList = await _dbContext.Matches
                .Where(m => m.League.Name.ToLower().Equals(leagueName.ToLower()) && m.LeagueSeason.Year == season)
                .OrderBy(m => m.Date)
                .ThenBy(m => m.Round)
                .Select(m => m.Round)
                .ToListAsync();

            var distinctRoundList = nonDistinctRoundList
                .GroupBy(r => r)
                .Select(r => r.First());

            return distinctRoundList;
        }

        public async Task<IEnumerable<Match>> GetMatchesForLeague(string leagueName, int? season, string round)
        {
            var query = _dbContext.Matches
               .Where(m => m.League.Name.ToLower().Equals(leagueName.ToLower()));

            if (season != null)
                query = query.Where(m => m.LeagueSeason.Year == season);

            if (round != null)
                query = query.Where(m => m.Round.ToLower().Equals(round.ToLower()));

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
               .ToListAsync();
        }

        public async Task<IEnumerable<LeagueStanding>> GetStandingsForLeagueAndSeason(string leagueName, int season)
        {
            return await _dbContext.LeagueStandings
               .Where(ls => ls.League.Name.ToLower().Equals(leagueName.ToLower()) && ls.LeagueSeason.Year == season)
               .OrderBy(s => s.Group)
               .ThenBy(s => s.Rank)
               .Include(s => s.Team)
               .ToListAsync();
        }

        public async Task<IEnumerable<TopScorer>> GetTopScorersForLeagueAndSeason(string leagueName, int season)
        {
            return await _dbContext.TopScorers
               .Where(t => t.League.Name.ToLower().Equals(leagueName.ToLower()) && t.LeagueSeason.Year == season)
               .OrderBy(t => t.Rank)
               .Include(t => t.Team)
               .ToListAsync();
        }

        public async Task<IEnumerable<League>> Search(string leagueName, string country, string type, int? currentSeason)
        {
            IQueryable<League> query = _dbContext.Leagues;
            if (!String.IsNullOrEmpty(leagueName))
            {
                query = query.Where(l => l.Name.ToLower().Contains(leagueName.ToLower()));
            }
            if (!String.IsNullOrEmpty(country))
            {
                query = query.Where(l => l.Country.Name.ToLower().Contains(country.ToLower()));
            }
            if (!String.IsNullOrEmpty(type))
            {
                query = query.Where(l => l.Type.ToLower().Equals(type.ToLower()));
            }
            if (currentSeason != null)
            {
                query = query.Where(l => l.CurrentSeason.Year == currentSeason);
            }

            return await query.ToListAsync();
        }
    }
}
