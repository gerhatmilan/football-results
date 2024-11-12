using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.DataAccess.Repositories.Football;
using System.Text.Json;

namespace FootballResults.WebApp.Services.Football.Server
{
    public class LeagueServiceServer : ILeagueService
    {
        private readonly ILeagueRepository _leagueRepository;
        private readonly ICountryRepository _countryRepository;

        public LeagueServiceServer(ILeagueRepository leagueRepository, ICountryRepository countryRepository)
        {
            _leagueRepository = leagueRepository;
            _countryRepository = countryRepository;
        }

        public async Task<League?> GetLeagueByNameAsync(string leagueName, bool tracking = true)
        {
            return await _leagueRepository.GetLeagueByName(leagueName, tracking);
        }

        public async Task<IEnumerable<League>> GetLeaguesAsync(bool tracking = true)
        {
            return await _leagueRepository.GetAllAsync(tracking);
        }

        public async Task<IEnumerable<Country>> GetCountriesWithLeaguesAsync(bool tracking = true)
        {
            return await _countryRepository.GetLeaguesByCountry(tracking);
        }

        public async Task<IEnumerable<LeagueStanding>> GetStandingsForLeagueAndSeasonAsync(string leagueName, int season, bool tracking = true)
        {
            return await _leagueRepository.GetStandingsForLeagueAndSeason(leagueName, season, tracking);
        }

        public async Task<IEnumerable<TopScorer>> GetTopScorersForLeagueAndSeasonAsync(string leagueName, int season, bool tracking = true)
        {
            return await _leagueRepository.GetTopScorersForLeagueAndSeason(leagueName, season, tracking);
        }

        public async Task<IEnumerable<League>> SearchAsync(string leagueName, bool tracking = true)
        {
            return await _leagueRepository.Search(leagueName, null, null, null, tracking);
        }

    }
}