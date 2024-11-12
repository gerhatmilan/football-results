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

        public async Task<Team?> GetTeamByNameAsync(string teamName, bool tracking = true)
        {
            return await _teamRepository.GetTeamByName(teamName, tracking);
        }

        public async Task<IEnumerable<Country>> GetCountriesWithTeamsAsync(bool tracking = true)
        {
            return await _countryRepository.GetTeamsByCountry(tracking);
        }

        public async Task<IEnumerable<Player>> GetSquadForTeamAsync(string teamName, bool tracking = true)
        {
            return await _teamRepository.GetSquadForTeam(teamName, tracking);
        }

        public async Task<IEnumerable<Team>> SearchAsync(string teamName, bool tracking = true)
        {
            return await _teamRepository.Search(teamName, null, null, tracking);
        }
    }
}
