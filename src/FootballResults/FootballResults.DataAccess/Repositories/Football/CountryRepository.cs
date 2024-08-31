using FootballResults.DataAccess.Entities.Football;
using Microsoft.EntityFrameworkCore;

namespace FootballResults.DataAccess.Repositories.Football
{
    public class CountryRepository : GenericRepository<Country>, ICountryRepository
    {
        public CountryRepository(AppDbContext dbContext) : base(dbContext) { }
        
        public async Task<IEnumerable<Country>> GetLeaguesByCountry()
        {
            return await _dbContext.Countries
                .Include(c => c.Leagues)
                .Where(c => c.Leagues.Count() > 0)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Country>> GetTeamsByCountry()
        {
            return await _dbContext.Countries
                .Include(c => c.Teams)
                .Where(c => c.Teams.Count() > 0)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Country>> GetVenuesByCountry()
        {
            return await _dbContext.Countries
                .Include(c => c.Venues)
                .Where(c => c.Venues.Count() > 0)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Country> GetCountryByName(string countryName)
        {
            return await _dbContext.Countries
                .FirstOrDefaultAsync(c => c.Name.ToLower().Equals(countryName.ToLower()));
        }

        public async Task<IEnumerable<League>> GetLeaguesForCountry(string countryName)
        {
            return await _dbContext.Leagues
                .Where(l => l.Country.Name.ToLower().Equals(countryName.ToLower()))
                .OrderBy(l => l.Type)
                .ThenBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Team>> GetTeamsForCountry(string countryName)
        {
            return await _dbContext.Teams
                .Where(t => t.Country.Name.ToLower().Equals(countryName.ToLower()))
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Venue>> GetVenuesForCountry(string countryName)
        {
            return await _dbContext.Venues
                .Where(v => v.Country.Name.ToLower().Equals(countryName.ToLower()))
                .OrderBy(v => v.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Country>> Search(string countryName)
        {
            IQueryable<Country> query = _dbContext.Countries;
            if (!String.IsNullOrEmpty(countryName))
            {
                query = query.Where(c => c.Name.ToLower().Contains(countryName.ToLower()));
            }

            return await query.ToListAsync();
        }
    }
}
