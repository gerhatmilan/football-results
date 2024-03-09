using FootballResults.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System;

namespace FootballResults.API.Models
{
    public class CountryRepository : ICountryRepository
    {
        private readonly FootballDataDbContext dbContext;

        public CountryRepository(FootballDataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Country>> GetCountries()
        {
            var result = await dbContext.Countries
                .OrderBy(c => c.CountryID)
                .ToListAsync();

            return result;
        }
        
        public async Task<IEnumerable<Country>> GetLeaguesByCountry()
        {
            var result = await dbContext.Countries
                .Include(c => c.Leagues)
                .Where(c => c.Leagues.Count > 0)
                .OrderBy(c => c.CountryID)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<Country>> GetTeamsByCountry()
        {
            var result = await dbContext.Countries
                .Include(c => c.Teams)
                .Where(c => c.Teams.Count > 0)
                .OrderBy(c => c.CountryID)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<Country>> GetVenuesByCountry()
        {
            var result = await dbContext.Countries
                .Include(c => c.Venues)
                .Where(c => c.Venues.Count > 0)
                .OrderBy(c => c.CountryID)
                .ToListAsync();

            return result;
        }

        public async Task<Country?> GetCountryByName(string countryName)
        {
            var result = await dbContext.Countries
                .FirstOrDefaultAsync(c => c.CountryID.ToLower().Equals(countryName.ToLower()));

            return result;
        }

        public async Task<IEnumerable<League>> GetLeaguesInCountry(string countryName)
        {
            var result = await dbContext.Leagues
                .Where(l => l.CountryID.ToLower().Equals(countryName.ToLower()))
                .OrderBy(l => l.Type)
                .ThenBy(l => l.Name)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<Team>> GetTeamsInCountry(string countryName)
        {
            var result = await dbContext.Teams
                .Where(t => t.CountryID.ToLower().Equals(countryName.ToLower()))
                .OrderBy(t => t.Name)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<Venue>> GetVenuesInCountry(string countryName)
        {
            var result = await dbContext.Venues
                .Where(v => v.CountryID.ToLower().Equals(countryName.ToLower()))
                .OrderBy(v => v.Name)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<Country>> Search(string? countryName)
        {
            IQueryable<Country> query = dbContext.Countries;
            if (!String.IsNullOrEmpty(countryName))
            {
                query = query.Where(c => c.CountryID.ToLower().Contains(countryName.ToLower()));
            }

            return await query.ToListAsync();
        }
    }
}
