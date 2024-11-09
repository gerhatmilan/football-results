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
                .Where(c => c.Leagues.Count() > 0)
                .Include(c => c.Leagues)
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Country>> GetTeamsByCountry()
        {
            return await _dbContext.Countries
                .Where(c => c.Teams.Count() > 0)
                .Include(c => c.Teams)
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Country>> GetVenuesByCountry()
        {
            return await _dbContext.Countries
                .Where(c => c.Venues.Count() > 0)
                .Include(c => c.Venues)
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Country> GetCountryByName(string countryName)
        {
            return await _dbContext.Countries
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name.ToLower().Equals(countryName.ToLower()));
        }

        public async Task<IEnumerable<League>> GetLeaguesForCountry(string countryName)
        {
            Country country = await _dbContext.Countries.FirstOrDefaultAsync(c => c.Name.ToLower().Equals(countryName.ToLower()));

            return country != null
                ? await _dbContext.Leagues
                    .Where(l => l.Country.Name.ToLower().Equals(country.Name.ToLower()))
                    .OrderBy(l => l.Type)
                    .ThenBy(l => l.Name)
                    .AsNoTracking()
                    .ToListAsync()
                : Enumerable.Empty<League>();
        }

        public async Task<IEnumerable<Team>> GetTeamsForCountry(string countryName)
        {
            Country country = await _dbContext.Countries.FirstOrDefaultAsync(c => c.Name.ToLower().Equals(countryName.ToLower()));

            return country != null
                ? await _dbContext.Teams
                    .Where(t => t.Country.Name.ToLower().Equals(country.Name.ToLower()))
                    .OrderBy(t => t.Name)
                    .AsNoTracking()
                    .ToListAsync()
                : Enumerable.Empty<Team>();
        }

        public async Task<IEnumerable<Venue>> GetVenuesForCountry(string countryName)
        {
            Country country = await _dbContext.Countries.FirstOrDefaultAsync(c => c.Name.ToLower().Equals(countryName.ToLower()));

            return country != null
                ? await _dbContext.Venues
                    .Where(v => v.Country.Name.ToLower().Equals(country.Name.ToLower()))
                    .OrderBy(v => v.Name)
                    .AsNoTracking()
                    .ToListAsync()
                : Enumerable.Empty<Venue>();
        }

        public async Task<IEnumerable<Country>> Search(string countryName)
        {
            IQueryable<Country> query = _dbContext.Countries;
            if (!String.IsNullOrEmpty(countryName))
            {
                query = query.Where(c => c.Name.ToLower().Contains(countryName.ToLower()));
            }

            return await query.AsNoTracking().ToListAsync();
        }
    }
}
