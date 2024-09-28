using FootballResults.API.Models;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.DataAccess.Repositories.Football;

namespace FootballResults.WebApp.Services.Football.Server
{
    public class TeamServiceServer : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ICountryRepository _countryRepository;

        public TeamServiceServer(ITeamRepository teamRepository, ICountryRepository countryRepository)
        {
            _teamRepository = teamRepository;
            _countryRepository = countryRepository;
        }

        public async Task<Team?> GetTeamByNameAsync(string teamName)
        {
            return await _teamRepository.GetTeamByName(teamName);
        }

        public async Task<IEnumerable<Country>> GetCountriesWithTeamsAsync()
        {
            return await _countryRepository.GetTeamsByCountry();
        }


        public async Task<IEnumerable<Player>> GetSquadForTeamAsync(string teamName)
        {
            return await _teamRepository.GetSquadForTeam(teamName);
        }

        public async Task<IEnumerable<Team>> SearchAsync(string teamName)
        {
            return await _teamRepository.Search(teamName, null, null);
        }

        public IEnumerable<Team> GetTeamsFavoritesFirst(User user, IEnumerable<Team> teams)
        {
            return teams.OrderByDescending(t => user.FavoriteTeams.Select(ft => ft.ID).Contains(t.ID)).ThenBy(t => t.Name);
        }
    }
}
