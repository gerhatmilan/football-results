using FootballResults.Models;
using Microsoft.AspNetCore.Mvc;

namespace FootballResults.WebApp.Services
{
    public interface IMatchService
    {
        Task<Match?> GetMatchByID(int id);
        Task<IEnumerable<Match>> GetHeadToHead(string teamName1, string teamName2);
        Task<IEnumerable<Match>> GetMatchesForToday();
        Task<IEnumerable<Match>> GetMatchesForDate(DateTime date);
        Task<IEnumerable<Match>> GetMatchesForLeagueAndDate(string leagueName, DateTime date);
        Task<IEnumerable<Match>> GetMatchesForTeamAndSeason(string teamName, int season);
        Task<IEnumerable<Match>> GetMatchesForLeagueAndSeason(string leagueName, int season);
        Task<IEnumerable<Match>> SearchForMatch(int? year, int? month, int? day, string? teamName, string? leagueName, int? season, string? round);
    }
}
