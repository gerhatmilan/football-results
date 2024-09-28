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

        public async Task<League?> GetLeagueByNameAsync(string leagueName)
        {
            return await _leagueRepository.GetLeagueByName(leagueName);
        }

        public async Task<IEnumerable<League>> GetLeaguesAsync()
        {
            return await _leagueRepository.GetAllAsync(tracking: false);
        }

        public async Task<IEnumerable<Country>> GetCountriesWithLeaguesAsync()
        {
            return await _countryRepository.GetLeaguesByCountry();
        }

        public async Task<IEnumerable<LeagueStanding>> GetStandingsForLeagueAndSeasonAsync(string leagueName, int season)
        {
            return await _leagueRepository.GetStandingsForLeagueAndSeason(leagueName, season);
        }

        public async Task<IEnumerable<TopScorer>> GetTopScorersForLeagueAndSeasonAsync(string leagueName, int season)
        {
           return await _leagueRepository.GetTopScorersForLeagueAndSeason(leagueName, season);
        }

        public async Task<IEnumerable<League>> SearchAsync(string leagueName)
        {
            return await _leagueRepository.Search(leagueName, null, null, null);
        }
        public IEnumerable<League> GetLeaguesFavoritesFirst(User user, IEnumerable<League> leagues)
        {
            return leagues.OrderByDescending(l => user.FavoriteLeagues.Select(fl => fl.ID).Contains(l.ID)).ThenBy(l => l.Name);
        }
    }
}