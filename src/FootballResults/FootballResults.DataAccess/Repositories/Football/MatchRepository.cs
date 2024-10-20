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

        public override async Task<Match> GetByIDAsync(int id, bool tracking)
        {
            Match match = await _dbContext.Matches
                .Include(m => m.LeagueSeason)
                    .ThenInclude(ls => ls.League)
                .Include(m => m.Venue)
                    .ThenInclude(v => v.Country)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (match != null)
            {
                return new Match
                {
                    ID = match.ID,
                    Date = match.Date,
                    VenueID = match.VenueID,
                    LeagueSeasonID = match.LeagueSeasonID,
                    Round = match.Round,
                    HomeTeamID = match.HomeTeamID,
                    AwayTeamID = match.AwayTeamID,
                    Status = match.Status,
                    Minute = match.Minute,
                    HomeTeamGoals = match.HomeTeamGoals,
                    AwayTeamGoals = match.AwayTeamGoals,
                    LeagueSeason = match.LeagueSeason,
                    Venue = match.Venue,
                    HomeTeam = match.HomeTeam,
                    AwayTeam = match.AwayTeam,
                    LastUpdate = match.LastUpdate,
                };
            }
            else
            {
                return null;
            }
        }

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
                .Include(m => m.LeagueSeason)
                .ThenInclude(ls => ls.League)
                .Include(m => m.Venue)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
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
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Match>> Search(DateTime? date, DateTime? from, DateTime? to, int? year, int? month, int? day, string teamName, string leagueName, int? season, string round)
        {
            return await _dbContext.Matches
                .Include(m => m.LeagueSeason)
                .ThenInclude(ls => ls.League)
                .Include(m => m.Venue)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => (!String.IsNullOrEmpty(teamName) ? m.HomeTeam.Name.ToLower().Equals(teamName.ToLower())
                        || m.AwayTeam.Name.ToLower().Equals(teamName.ToLower()) : true)
                    && (!String.IsNullOrEmpty(leagueName) ? m.LeagueSeason.League.Name.ToLower().Equals(leagueName.ToLower()) : true)
                    && (season.HasValue ? m.LeagueSeason.Year == season : true)
                    && (!String.IsNullOrEmpty(round) ? m.Round.ToLower().Equals(round.ToLower()) : true)
                    && (date.HasValue ? m.Date.Value.Date == DateTime.SpecifyKind(date.Value, DateTimeKind.Unspecified) : true)
                    && (from.HasValue ? m.Date.Value.Date >= DateTime.SpecifyKind(from.Value.Date, DateTimeKind.Unspecified) : true)
                    && (to.HasValue ? m.Date.Value.Date <= DateTime.SpecifyKind(to.Value.Date, DateTimeKind.Unspecified) : true)
                    && (year.HasValue ? m.Date.Value.Year == year : true)
                    && (month.HasValue ? m.Date.Value.Month == month : true)
                    && (day.HasValue ? m.Date.Value.Day == day : true))
               .AsNoTracking()
               .ToListAsync();
        }
    }
}
