using FootballResults.Models.Football;
using Microsoft.EntityFrameworkCore;

namespace FootballResults.API.Models
{
    public class MatchRepository : IMatchRepository
    {
        private readonly FootballDataDbContext dbContext;

        public MatchRepository(FootballDataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Match?> GetMatchByID(int id)
        {
            return await dbContext.Matches
                .Where(m => m.MatchID == id)
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
                    League = m.League,
                    Venue = m.Venue,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam,
                    LastUpdate = m.LastUpdate,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Match>> GetHeadToHead(string teamName1, string teamName2)
        {
            return await dbContext.Matches
                .Where(m =>
                    (m.HomeTeam.Name.ToLower().Equals(teamName1.ToLower())
                    && m.AwayTeam.Name.ToLower().Equals(teamName2.ToLower()))
                    ||
                    (m.HomeTeam.Name.ToLower().Equals(teamName2.ToLower())
                    && m.AwayTeam.Name.ToLower().Equals(teamName1.ToLower()))
                )
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
                    League = m.League,
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

        public async Task<IEnumerable<Match>> Search(DateTime? date, int? year, int? month, int? day, string? teamName, string? leagueName, int? season, string? round)
        {          
            IQueryable<Match> query = dbContext.Matches;

            if (!String.IsNullOrEmpty(teamName))
            {
                query = query.Where(m => m.HomeTeam.Name.ToLower().Equals(teamName.ToLower())
                    || m.AwayTeam.Name.ToLower().Equals(teamName.ToLower()));
            }
            if (!String.IsNullOrEmpty(leagueName))
            {
                query = query.Where(m => m.League.Name.ToLower().Equals(leagueName.ToLower()));
            }
            if (season != null)
            {
                query = query.Where(m => m.Season == season);
            }
            if (round != null)
            {
                query = query.Where(m => m.Round.ToLower().Equals(round.ToLower()));
            }

            query = query
                .OrderBy(m => m.Date)
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
                    League = m.League,
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
                });

            if (date.HasValue)
            {
                var dateToSearch = date.GetValueOrDefault().Date;
                query = query
                    .Where(m => m.Date != null && m.Date.Value.Date == dateToSearch);
            }

            if (year.HasValue)
            {
                query = query
                    .Where(m => m.Date != null && m.Date.Value.Year == year);
            }
            if (month.HasValue)
            {
                query = query
                    .Where(m => m.Date != null && m.Date.Value.Month == month);
            }
            if (day.HasValue)
            {
                query = query
                    .Where(m => m.Date != null && m.Date.Value.Day == day);
            }

            return await query.ToListAsync();
        }
    }
}
