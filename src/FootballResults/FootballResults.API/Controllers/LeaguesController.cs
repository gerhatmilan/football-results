using FootballResults.API.Models;
using FootballResults.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace FootballResults.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class LeaguesController : ControllerBase
    {
        private readonly ILeagueRepository leagueRepository;

        public LeaguesController(ILeagueRepository leagueRepository)
        {
            this.leagueRepository = leagueRepository;
        }

        [HttpGet("leagues")]
        public async Task<ActionResult<IEnumerable<League>>> GetLeagues()
        {
            try
            {
                var result = await leagueRepository.GetLeagues();
                return result.Any() ? Ok(result) : NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query leagues data");
            }
        } 

        [HttpGet("leagues/{leagueName}")]
        public async Task<ActionResult<League>> GetLeagueByName(string leagueName)
        {
            try
            {
                leagueName = leagueName.Replace("-", " ");

                var result = await leagueRepository.GetLeagueByName(leagueName);
                return result != null ? Ok(result) : NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query league data");
            }
        }

        [HttpGet("leagues/{leagueName}/seasons")]
        public async Task<ActionResult<IEnumerable<int>>> GetSeasonsForLeague(string leagueName)
        {
            try
            {
                leagueName = leagueName.Replace("-", " ");

                var result = await leagueRepository.GetSeasonsForLeague(leagueName);
                return result.Any() ? Ok(result) : NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query seasons data");
            }
        }

        [HttpGet("leagues/{leagueName}/{season}/teams")]
        public async Task<ActionResult<IEnumerable<Match>>> GetTeamsForLeagueAndSeason(string leagueName, int season)
        {
            try
            {
                leagueName = leagueName.Replace("-", " ");

                var result = await leagueRepository.GetTeamsForLeagueAndSeason(leagueName, season);
                return result.Any() ? Ok(result) : NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query teams data");
            }
        }

        [HttpGet("leagues/{leagueName}/{season}/rounds")]
        public async Task<ActionResult<IEnumerable<Match>>> GetRoundsForLeagueAndSeason(string leagueName, int season)
        {
            try
            {
                leagueName = leagueName.Replace("-", " ");

                var result = await leagueRepository.GetRoundsForLeagueAndSeason(leagueName, season);
                return result.Any() ? Ok(result) : NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query rounds data");
            }
        }

        [HttpGet("leagues/{leagueName}/matches")]
        public async Task<ActionResult<IEnumerable<Match>>> GetMatchesForLeagueAndSeason(string leagueName, int? season, string? round)
        {
            if (season == null || round == null)
                return BadRequest("Season and round has to be specified as query parameters to retrieve matches");

            try
            {
                leagueName = leagueName.Replace("-", " ");

                var result = await leagueRepository.GetMatchesForLeagueAndSeasonAndRound(leagueName, (int)season, round);
                return result.Any() ? Ok(result) : NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query seasons data");
            }
        }

        [HttpGet("leagues/{leagueName}/standings")]
        public async Task<ActionResult<IEnumerable<Match>>> GetStandingsForLeagueAndSeason(string league, int? season)
        {
            if (season == null)
                return BadRequest("Season has to be specified as query parameter to retrieve standings");

            try
            {
                league = league.Replace("-", " ");

                var result = await leagueRepository.GetStandingsForLeagueAndSeason(league, (int)season);
                return result.Any() ? Ok(result) : NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query seasons data");
            }
        }

        [HttpGet("leagues/{leagueName}/topscorers")]
        public async Task<ActionResult<IEnumerable<Match>>> GetTopScorersForLeagueAndSeason(string league, int? season)
        {
            if (season == null)
                return BadRequest("Season has to be specified as query parameter to retrieve top scorers");

            try
            {
                league = league.Replace("-", " ");

                var result = await leagueRepository.GetTopScorersForLeagueAndSeason(league, (int)season);
                return result.Any() ? Ok(result) : NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query seasons data");
            }
        }

        [HttpGet("leagues/search")]
        public async Task<ActionResult<IEnumerable<League>>> Search(string? league, string? country, string? type, int? currentSeason)
        {
            try
            {
                var result = await leagueRepository.Search(league, country, type, currentSeason);
                return result.Any() ? Ok(result) : NotFound();
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query leagues data");
            }
        }
    }
}
