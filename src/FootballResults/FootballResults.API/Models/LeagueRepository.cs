using FootballResults.Models.Football;
using Microsoft.EntityFrameworkCore;

namespace FootballResults.API.Models
{
    public class LeagueRepository : ILeagueRepository
    {
        private readonly FootballDataDbContext dbContext;
        public LeagueRepository(FootballDataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<League>> GetLeagues()
        {
            return await dbContext.Leagues
                .OrderBy(l => l.Type)
                .ThenBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<League?> GetLeagueByName(string leagueName)
        {
            return await dbContext.Leagues
                .FirstOrDefaultAsync(l => l.Name.ToLower().Equals(leagueName.ToLower()));
        }

        public async Task<IEnumerable<int>> GetSeasonsForLeague(string leagueName)
        {
            return await dbContext.AvailableSeasons
                .Where(s => s.League.Name.ToLower().Equals(leagueName.ToLower()))
                .OrderBy(s => s.Season)
                .Select(s => s.Season)
                .ToListAsync();
        }

        public async Task<IEnumerable<Team>> GetTeamsForLeague(string leagueName, int? season)
        {
            var query = dbContext.Matches
                .Where(m => m.League.Name.ToLower().Equals(leagueName.ToLower()));

            if (season != null)
                query = query.Where(m => m.Season == season);

            return await query
                .Select(m => m.HomeTeam)
                .Union(
                    query.Select(m=> m.AwayTeam)
                )
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetRoundsForLeagueAndSeason(string leagueName, int season)
        {

            var nonDistinctRoundList = await dbContext.Matches
                .Where(m => m.League.Name.ToLower().Equals(leagueName.ToLower()) && m.Season == season)
                .OrderBy(m => m.Date)
                .ThenBy(m => m.Round)
                .Select(m => m.Round)
                .ToListAsync();

            var distinctRoundList = nonDistinctRoundList
                .GroupBy(r => r)
                .Select(r => r.First());

            return distinctRoundList;
        }

        public async Task<IEnumerable<Match>> GetMatchesForLeague(string leagueName, int? season, string? round)
        {
            var query = dbContext.Matches
               .Where(m => m.League.Name.ToLower().Equals(leagueName.ToLower()));

            if (season != null)
                query = query.Where(m => m.Season == season);

            if (round != null)
                query = query.Where(m => m.Round.ToLower().Equals(round.ToLower()));

            return await query
               .OrderBy(m => m.Date)
               .Select(m => new Match
               {
                   MatchID = m.MatchID,
                   LeagueID = m.LeagueID,
                   HomeTeamID = m.HomeTeamID,
                   AwayTeamID = m.AwayTeamID,
                   Date = m.Date,
                   Season = m.Season,
                   Round = m.Round,
                   Status = m.Status,
                   Minute = m.Minute,
                   HomeTeamGoals = m.HomeTeamGoals,
                   AwayTeamGoals = m.AwayTeamGoals,
                   LastUpdate = m.LastUpdate,
                   HomeTeam = new Team
                   {
                       TeamID = m.HomeTeam.TeamID,
                       Name = m.HomeTeam.Name,
                       ShortName = m.HomeTeam.ShortName,
                       LogoLink = m.HomeTeam.LogoLink,
                   },
                   AwayTeam = new Team
                   {
                       TeamID = m.AwayTeam.TeamID,
                       Name = m.AwayTeam.Name,
                       ShortName = m.AwayTeam.ShortName,
                       LogoLink = m.AwayTeam.LogoLink,
                   }
               })
               .ToListAsync();
        }

        public async Task<IEnumerable<Standing>> GetStandingsForLeagueAndSeason(string leagueName, int season)
        {
            return await dbContext.Standings
               .Where(s => s.League.Name.ToLower().Equals(leagueName.ToLower()) && s.Season == season)
               .OrderBy(s => s.Group)
               .ThenBy(s => s.Rank)
               .Include(s => s.Team)
               .ToListAsync();
        }

        public async Task<IEnumerable<TopScorer>> GetTopScorersForLeagueAndSeason(string leagueName, int season)
        {
            return await dbContext.TopScorers
               .Where(t => t.League.Name.ToLower().Equals(leagueName.ToLower()) && t.Season == season)
               .OrderBy(t => t.Rank)
               .Include(t => t.Team)
               .ToListAsync();
        }

        public async Task<IEnumerable<League>> Search(string? leagueName, string? country, string? type, int? currentSeason)
        {
            IQueryable<League> query = dbContext.Leagues;
            if (!String.IsNullOrEmpty(leagueName))
            {
                query = query.Where(l => l.Name.ToLower().Contains(leagueName.ToLower()));
            }
            if (!String.IsNullOrEmpty(country))
            {
                query = query.Where(l => l.Country.CountryID.ToLower().Contains(country.ToLower()));
            }
            if (!String.IsNullOrEmpty(type))
            {
                query = query.Where(l => l.Type.ToLower().Equals(type.ToLower()));
            }
            if (currentSeason != null)
            {
                query = query.Where(l => l.CurrentSeason == currentSeason);
            }

            return await query.ToListAsync();
        }
    }
}
