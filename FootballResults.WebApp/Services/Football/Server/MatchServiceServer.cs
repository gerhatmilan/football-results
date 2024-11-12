using FootballResults.API.Models;
using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.DataAccess.Repositories.Football;
using FootballResults.Models.ViewModels.Football;

namespace FootballResults.WebApp.Services.Football.Server
{
    public class MatchServiceServer : IMatchService
    {
        private readonly IMatchRepository _matchRepository;

        public MatchServiceServer(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        public async Task<Match?> GetMatchByIDAsync(int id, bool tracking = true)
        {
            return await _matchRepository.GetByIDAsync(id, tracking);
        }

        public async Task<IEnumerable<Match>> GetHeadToHeadAsync(string teamName1, string teamName2, bool tracking = true)
        {
            return await _matchRepository.GetHeadToHead(teamName1, teamName2, tracking);
        }

        public async Task<IEnumerable<Match>> GetMatchesForDateAsync(DateTime date, bool tracking = true)
        {
            return await SearchForMatchAsync(date: date, tracking: tracking);
        }

        public async Task<IEnumerable<Match>> GetMatchesForIntervalAsync(DateTime from, DateTime to, bool tracking = true)
        {
            return await SearchForMatchAsync(from: from, to: to, tracking: tracking);
        }

        public async Task<IEnumerable<Match>> GetMatchesForYearAsync(int year, bool tracking = true)
        {
            return await SearchForMatchAsync(year: year, tracking: tracking);
        }

        public async Task<IEnumerable<Match>> GetMatchesForLeagueAndDateAsync(string leagueName, DateTime date, bool tracking = true)
        {
            return await SearchForMatchAsync(leagueName: leagueName, date: date, tracking: tracking);
        }

        public async Task<IEnumerable<Match>> GetMatchesForLeagueAndSeasonAsync(string leagueName, int season, bool tracking = true)
        {
            return await SearchForMatchAsync(leagueName: leagueName, season: season, tracking: tracking);
        }

        public async Task<IEnumerable<Match>> GetMatchesForTeamAndSeasonAsync(string teamName, int season, bool tracking = true)
        {
            return await SearchForMatchAsync(teamName: teamName, season: season, tracking: tracking);
        }

        public async Task<IEnumerable<Match>> SearchForMatchAsync(DateTime? date = null, DateTime? from = null, DateTime? to = null,
            int? year = null, int? month = null, int? day = null, string? teamName = null, string? leagueName = null, int? season = null, string? round = null, bool tracking = true)
        {
            return await _matchRepository.Search(date, from, to, year, month, day, teamName, leagueName, season, round, tracking);
        }
    }
}