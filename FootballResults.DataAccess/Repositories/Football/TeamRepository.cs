using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Repositories;
using FootballResults.DataAccess.Repositories.Football;
using Microsoft.EntityFrameworkCore;


namespace FootballResults.API.Models
{
    public class TeamRepository : GenericRepository<Team>, ITeamRepository
    {
        public TeamRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<Team> GetTeamByName(string teamName, bool tracking = true)
        {
            return await _dbContext.Teams
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .FirstOrDefaultAsync(l => l.Name.ToLower().Equals(teamName.ToLower()));
        }

        public async Task<IEnumerable<Player>> GetSquadForTeam(string teamName, bool tracking = true)
        {
            Team team = await _dbContext.Teams
                .Include(t => t.Squad)
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .FirstOrDefaultAsync(t => t.Name.ToLower().Equals(teamName.ToLower()));

            if (team != null && team.Squad != null)
            {
                return team.Squad
                    .OrderByDescending(p => p.Number.HasValue)
                    .ThenBy(p => p.Number);
            }
            else
            {
                return Enumerable.Empty<Player>();
            }
        }

        public async Task<IEnumerable<Match>> GetMatchesForTeamAndLeagueAndSeason(string teamName, string leagueName, int season, bool tracking = true)
        {
            Team team = await _dbContext.Teams
                .Include(t => t.HomeMatches).ThenInclude(m => m.LeagueSeason).ThenInclude(ls => ls.League)
                .Include(t => t.HomeMatches).ThenInclude(m => m.Venue)
                .Include(t => t.HomeMatches).ThenInclude(m => m.AwayTeam)
                .Include(t => t.AwayMatches).ThenInclude(m => m.LeagueSeason).ThenInclude(ls => ls.League)
                .Include(t => t.AwayMatches).ThenInclude(m => m.Venue)
                .Include(t => t.AwayMatches).ThenInclude(m => m.HomeTeam)
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .FirstOrDefaultAsync(t => t.Name.ToLower().Equals(teamName.ToLower()));

            if (team != null)
            {
                return team.Matches
                    .Where(m => m.LeagueSeason.League.Name.ToLower().Equals(leagueName.ToLower()) && m.LeagueSeason.Year == season)
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
                        LeagueSeason = m.LeagueSeason,
                        Venue = m.Venue,
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
                    .ToList();
            }
            else
            {
                return Enumerable.Empty<Match>();
            }
        }

        public async Task<IEnumerable<Team>> Search(string teamName, string country, bool? national, bool tracking = true)
        {
            return await _dbContext.Teams
                .Where(t => (!String.IsNullOrEmpty(teamName) ? t.Name.ToLower().Contains(teamName.ToLower()) : true)
                    && (!String.IsNullOrEmpty(country) ? t.Country.Name.ToLower().Contains(country.ToLower()) : true)
                    && (national.HasValue ? t.National == national : true))
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .ToListAsync();
        }
    }
}
