using FootballResults.Models.Football;
using Microsoft.EntityFrameworkCore;


namespace FootballResults.API.Models
{
    public class TeamRepository : ITeamRepository
    {
        private readonly FootballDataDbContext dbContext;

        public TeamRepository(FootballDataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Team>> GetTeams()
        {
            return await dbContext.Teams
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<Team?> GetTeamByName(string teamName)
        {
            return await dbContext.Teams
                .FirstOrDefaultAsync(l => l.Name.ToLower().Equals(teamName.ToLower()));
        }

        public async Task<IEnumerable<Player>> GetSquadForTeam(string teamName)
        {
            var squad = await dbContext.Teams
                .Where(t => t.Name.ToLower().Equals(teamName.ToLower()))
                .Include(t => t.Squad)
                .Select(t => t.Squad)
                .FirstOrDefaultAsync();

            if (squad == null)
                return new List<Player>();
            else {
                return squad
                    .OrderByDescending(p => p.Number.HasValue)
                    .ThenBy(p => p.Number);
            }
        }

        public async Task<IEnumerable<Match>> GetMatchesForTeamAndLeagueAndSeason(string teamName, string leagueName, int season)
        {
            var query = dbContext.Matches
                .Where(m => m.Season == season)
                .Where(m => m.League.Name.ToLower().Contains(leagueName.ToLower()))
                .Where(m => m.HomeTeam.Name.ToLower().Equals(teamName.ToLower())
                || m.AwayTeam.Name.ToLower().Equals(teamName.ToLower()));

            return await query
                .Select(m => new Match
                {
                    MatchID = m.MatchID,
                    Date = m.Date,
                    VenueID = m.VenueID,
                    LeagueID = m.LeagueID,
                    Season = m.Season,
                    Round = m.Round,
                    HomeTeamID = m.HomeTeamID,
                    AwayTeamID = m.AwayTeamID,
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
                    },
                })
                .OrderBy(m => m.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Team>> Search(string? teamName, string? country, bool? national)
        {
            IQueryable<Team> query = dbContext.Teams;
            if (!String.IsNullOrEmpty(teamName))
            {
                query = query.Where(t => t.Name.ToLower().Contains(teamName.ToLower()));
            }
            if (!String.IsNullOrEmpty(country))
            {
                query = query.Where(t => t.CountryID.ToLower().Contains(country.ToLower()));
            }
            if (national != null)
            {
                query = query.Where(t => t.National == national);
            }

            return await query.ToListAsync();
        }
    }
}
