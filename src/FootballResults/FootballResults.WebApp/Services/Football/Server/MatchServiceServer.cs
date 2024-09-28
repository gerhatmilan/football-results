using FootballResults.API.Models;
using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.DataAccess.Repositories.Football;

namespace FootballResults.WebApp.Services.Football.Server
{
    public class MatchServiceServer : IMatchService
    {
        private readonly IMatchRepository _matchRepository;

        public MatchServiceServer(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        public async Task<Match?> GetMatchByIDAsync(int id)
        {
            return await _matchRepository.GetByIDAsync(id, tracking: false);
        }
        public async Task<IEnumerable<Match>> GetHeadToHeadAsync(string teamName1, string teamName2)
        {
            return await _matchRepository.GetHeadToHead(teamName1, teamName2);
        }

        public async Task<IEnumerable<Match>> GetMatchesForDateAsync(DateTime date)
        {
            return await SearchForMatchAsync(date: date);
        }

        public async Task<IEnumerable<Match>> GetMatchesForIntervalAsync(DateTime from, DateTime to)
        {
            return await SearchForMatchAsync(from: from, to: to);
        }

        public async Task<IEnumerable<Match>> GetMatchesForYearAsync(int year)
        {
            return await SearchForMatchAsync(year: year);
        }

        public async Task<IEnumerable<Match>> GetMatchesForLeagueAndDateAsync(string leagueName, DateTime date)
        {
            return await SearchForMatchAsync(leagueName: leagueName, date: date);
        }

        public async Task<IEnumerable<Match>> GetMatchesForLeagueAndSeasonAsync(string leagueName, int season)
        {
            return await SearchForMatchAsync(leagueName: leagueName, season: season);
        }

        public async Task<IEnumerable<Match>> GetMatchesForTeamAndSeasonAsync(string teamName, int season)
        {
            return await SearchForMatchAsync(teamName: teamName, season: season);
        }

        public async Task<IEnumerable<Match>> SearchForMatchAsync(DateTime? date = null, DateTime? from = null, DateTime? to = null,
            int? year = null, int? month = null, int? day = null, string? teamName = null, string? leagueName = null, int? season = null, string? round = null)
        {
            return await _matchRepository.Search(date, from, to, year, month, day, teamName, leagueName, season, round);
        }

        public IEnumerable<(int leagueID, List<Match> matches)> GetMatchesGroupedByLeague(IEnumerable<Match> matches)
        {
            return matches
            .GroupBy(
                m => m.LeagueSeason.LeagueID,
                (leagueID, matches) => (leagueID, matches!.Where(m => m.LeagueSeason.LeagueID.Equals(leagueID))
                    .OrderBy(m => m.Date).ToList())
            )
            .OrderBy(pair => pair.Item2.ElementAt(0).League.CountryID) // order by country name of the league
            .ThenBy(pair => pair.Item2.ElementAt(0).League.Name) // then by league name
            .ToList();
        }

        public IEnumerable<(int leagueID, List<Match> matches)> GetMatchesGroupedByLeagueFavoritesFirst(User user, IEnumerable<Match> matches)
        {
            var groupedMatches = GetMatchesGroupedByLeague(matches);

            return groupedMatches
                .OrderByDescending(pair => user.FavoriteLeagues.Select(fl => fl.ID).Contains(pair.leagueID)); // favorite leagues first
        }
    }
}