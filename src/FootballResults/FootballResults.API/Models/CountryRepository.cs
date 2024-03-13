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
            return await dbContext.Countries
                .OrderBy(c => c.CountryID)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Country>> GetLeaguesByCountry()
        {
            return await dbContext.Countries
                .Include(c => c.Leagues)
                .Where(c => c.Leagues.Count > 0)
                .OrderBy(c => c.CountryID)
                .ToListAsync();
        }

        public async Task<IEnumerable<Country>> GetTeamsByCountry()
        {
            return await dbContext.Countries
                .Include(c => c.Teams)
                .Where(c => c.Teams.Count > 0)
                .OrderBy(c => c.CountryID)
                .ToListAsync();
        }

        public async Task<IEnumerable<Country>> GetVenuesByCountry()
        {
            return await dbContext.Countries
                .Include(c => c.Venues)
                .Where(c => c.Venues.Count > 0)
                .OrderBy(c => c.CountryID)
                .ToListAsync();
        }

        public async Task<Country?> GetCountryByName(string countryName)
        {
            return await dbContext.Countries
                .FirstOrDefaultAsync(c => c.CountryID.ToLower().Equals(countryName.ToLower()));
        }

        public async Task<IEnumerable<League>> GetLeaguesForCountry(string countryName)
        {
            return await dbContext.Leagues
                .Where(l => l.CountryID.ToLower().Equals(countryName.ToLower()))
                .OrderBy(l => l.Type)
                .ThenBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Team>> GetTeamsForCountry(string countryName)
        {
            return await dbContext.Teams
                .Where(t => t.CountryID.ToLower().Equals(countryName.ToLower()))
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Venue>> GetVenuesForCountry(string countryName)
        {
            return await dbContext.Venues
                .Where(v => v.CountryID.ToLower().Equals(countryName.ToLower()))
                .OrderBy(v => v.Name)
                .ToListAsync();
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
