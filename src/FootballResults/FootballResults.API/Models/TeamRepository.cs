using FootballResults.Models;
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
            var result = await dbContext.Teams
                .OrderBy(t => t.Name)
                .ToListAsync();

            return result;
        }

        public async Task<Team?> GetTeamByName(string teamName)
        {
            var result = await dbContext.Teams
                .FirstOrDefaultAsync(l => l.Name.ToLower().Equals(teamName.ToLower()));

            return result;
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
