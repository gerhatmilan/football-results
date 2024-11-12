using FootballResults.DataAccess.Entities.Football;
using Microsoft.EntityFrameworkCore;

namespace FootballResults.DataAccess.Repositories.Football
{
    public class CountryRepository : GenericRepository<Country>, ICountryRepository
    {
        public CountryRepository(AppDbContext dbContext) : base(dbContext) { }
        
        public async Task<IEnumerable<Country>> GetLeaguesByCountry(bool tracking = true)
        {
            return await _dbContext.Countries
                .Include(c => c.Leagues)
                .Where(c => c.Leagues.Count() > 0)
                .OrderBy(c => c.Name)
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .ToListAsync();
        }

        public async Task<IEnumerable<Country>> GetTeamsByCountry(bool tracking = true)
        {
            return await _dbContext.Countries
                .Where(c => c.Teams.Count() > 0)
                .Include(c => c.Teams)
                .OrderBy(c => c.Name)
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .ToListAsync();
        }

        public async Task<IEnumerable<Country>> GetVenuesByCountry(bool tracking = true)
        {
            return await _dbContext.Countries
                .Where(c => c.Venues.Count() > 0)
                .Include(c => c.Venues)
                .OrderBy(c => c.Name)
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .ToListAsync();
        }

        public async Task<Country> GetCountryByName(string countryName, bool tracking = true)
        {
            return await _dbContext.Countries
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .FirstOrDefaultAsync(c => c.Name.ToLower().Equals(countryName.ToLower()));
        }

        public async Task<IEnumerable<League>> GetLeaguesForCountry(string countryName, bool tracking = true)
        {
            Country country = await _dbContext.Countries.FirstOrDefaultAsync(c => c.Name.ToLower().Equals(countryName.ToLower()));

            return country != null
                ? await _dbContext.Leagues
                    .Where(l => l.Country.Name.ToLower().Equals(country.Name.ToLower()))
                    .OrderBy(l => l.Type)
                    .ThenBy(l => l.Name)
                    .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                    .ToListAsync()
                : Enumerable.Empty<League>();
        }

        public async Task<IEnumerable<Team>> GetTeamsForCountry(string countryName, bool tracking = true)
        {
            Country country = await _dbContext.Countries.FirstOrDefaultAsync(c => c.Name.ToLower().Equals(countryName.ToLower()));

            return country != null
                ? await _dbContext.Teams
                    .Where(t => t.Country.Name.ToLower().Equals(country.Name.ToLower()))
                    .OrderBy(t => t.Name)
                    .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                    .ToListAsync()
                : Enumerable.Empty<Team>();
        }

        public async Task<IEnumerable<Venue>> GetVenuesForCountry(string countryName, bool tracking = true)
        {
            Country country = await _dbContext.Countries.FirstOrDefaultAsync(c => c.Name.ToLower().Equals(countryName.ToLower()));

            return country != null
                ? await _dbContext.Venues
                    .Where(v => v.Country.Name.ToLower().Equals(country.Name.ToLower()))
                    .OrderBy(v => v.Name)
                    .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                    .ToListAsync()
                : Enumerable.Empty<Venue>();
        }

        public async Task<IEnumerable<Country>> Search(string countryName, bool tracking = true)
        {
            IQueryable<Country> query = _dbContext.Countries;
            if (!String.IsNullOrEmpty(countryName))
            {
                query = query.Where(c => c.Name.ToLower().Contains(countryName.ToLower()));
            }

            return await query
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .ToListAsync();
        }
    }
}
