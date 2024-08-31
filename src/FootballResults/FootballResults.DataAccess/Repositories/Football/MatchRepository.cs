using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Repositories;
using FootballResults.DataAccess.Repositories.Football;
using Microsoft.EntityFrameworkCore;

namespace FootballResults.API.Models
{
    public class MatchRepository : GenericRepository<Match>, IMatchRepository
    {
        public MatchRepository(AppDbContext dbContext) : base(dbContext) { }
        
        public async Task<IEnumerable<Match>> GetHeadToHead(string teamName1, string teamName2)
        {
            return await _dbContext.Matches
                .Where(m =>
                    (m.HomeTeam.Name.ToLower().Equals(teamName1.ToLower())
                    && m.AwayTeam.Name.ToLower().Equals(teamName2.ToLower()))
                    ||
                    (m.HomeTeam.Name.ToLower().Equals(teamName2.ToLower())
                    && m.AwayTeam.Name.ToLower().Equals(teamName1.ToLower()))
                )
                .Select(m => new Match
                {
                    ID = m.ID,
                    Date = m.Date,
                    VenueID = m.VenueID,
                    LeagueSeasonID = m.LeagueSeasonID,
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
                    },
                })
                .OrderBy(m => m.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Match>> Search(DateTime? date, int? year, int? month, int? day, string teamName, string leagueName, int? season, string round)
        {          
            IQueryable<Match> query = _dbContext.Matches;

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
                query = query.Where(m => m.LeagueSeason.Year == season);
            }
            if (round != null)
            {
                query = query.Where(m => m.Round.ToLower().Equals(round.ToLower()));
            }

            query = query
                .OrderBy(m => m.Date)
                .Select(m => new Match
                {
                    ID = m.ID,
                    Date = m.Date,
                    VenueID = m.VenueID,
                    LeagueSeasonID = m.LeagueSeasonID,
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
