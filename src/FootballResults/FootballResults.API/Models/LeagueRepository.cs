using FootballResults.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Xml.Linq;

namespace FootballResults.API.Models
{
    public class LeagueRepository : ILeagueRepository
    {
        private readonly FootballDataDbContext dbContext;
        private const int MATCH_PAGE_SIZE = 20;

        public LeagueRepository(FootballDataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<League>> GetLeagues()
        {
            var result = await dbContext.Leagues
                .OrderBy(l => l.Type)
                .ThenBy(l => l.Name)
                .ToListAsync();

            return result;
        }

        public async Task<League?> GetLeagueByName(string leagueName)
        {
            var result = await dbContext.Leagues
                .FirstOrDefaultAsync(l => l.Name.ToLower().Equals(leagueName.ToLower()));

            return result;
        }

        public async Task<IEnumerable<int>> GetSeasonsForLeague(string leagueName)
        {

            var result = await dbContext.AvailableSeasons
                .Where(s => s.League.Name.ToLower().Equals(leagueName.ToLower()))
                .OrderBy(s => s.Season)
                .Select(s => s.Season)
                .ToListAsync();

            return result;
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

        public async Task<IEnumerable<Match>> GetMatchesForLeagueAndSeasonAndRound(string leagueName, int season, string round)
        {
            var result = await dbContext.Matches
               .Where(m => m.League.Name.ToLower().Equals(leagueName.ToLower())
                    && m.Season == season
                    && m.Round.ToLower().Equals(round.ToLower()))
               .OrderBy(m => m.Date)
               .Include(m => m.Venue)
               .Include(m => m.League)
               .Select(m => new Match
               {
                   MatchID = m.MatchID,
                   LeagueID = m.LeagueID,
                   HomeTeamID = m.HomeTeamID,
                   AwayTeamID = m.AwayTeamID,
                   Date = m.Date,
                   Venue = m.Venue,
                   Season = m.Season,
                   Round = m.Round,
                   HomeTeam = new Team
                   {
                       TeamID = m.HomeTeam.TeamID,
                       Name = m.HomeTeam.Name,
                       ShortName = m.HomeTeam.ShortName,
                       LogoLink = m.HomeTeam.LogoLink,
                       National = m.HomeTeam.National,
                   },
                   AwayTeam = new Team
                   {
                       TeamID = m.AwayTeam.TeamID,
                       Name = m.AwayTeam.Name,
                       ShortName = m.AwayTeam.ShortName,
                       LogoLink = m.AwayTeam.LogoLink,
                       National = m.AwayTeam.National,
                   },
                   Status = m.Status,
                   Minute = m.Minute,
                   HomeTeamGoals = m.HomeTeamGoals,
                   AwayTeamGoals = m.AwayTeamGoals,
                   LastUpdate = m.LastUpdate
               })
               .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<Standing>> GetStandingsForLeagueAndSeason(string leagueName, int season)
        {
            var result = await dbContext.Standings
               .Where(s => s.League.Name.ToLower().Equals(leagueName.ToLower()) && s.Season == season)
               .OrderBy(s => s.Group)
               .ThenBy(s => s.Rank)
               .Include(s => s.Team)
               .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<TopScorer>> GetTopScorersForLeagueAndSeason(string leagueName, int season)
        {
            var result = await dbContext.TopScorers
               .Where(t => t.League.Name.ToLower().Equals(leagueName.ToLower()) && t.Season == season)
               .OrderBy(t => t.Rank)
               .Include(t => t.Team)
               .ToListAsync();

            return result;
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
