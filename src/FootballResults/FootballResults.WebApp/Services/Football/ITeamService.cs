using FootballResults.Models.Football;

namespace FootballResults.WebApp.Services.Football
{
    public interface ITeamService
    {
        Task<Team?> GetTeamByNameAsync(string teamName);
        Task<IEnumerable<Country>> GetCountriesWithTeamsAsync();
        Task<IEnumerable<Player>> GetSquadForTeamAsync(string teamName);
        Task<IEnumerable<Team>> SearchAsync(string teamName);
    }
}
