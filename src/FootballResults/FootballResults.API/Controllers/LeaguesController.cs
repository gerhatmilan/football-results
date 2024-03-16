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
                return Ok(result);
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
                var result = await leagueRepository.GetSeasonsForLeague(leagueName);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query seasons data");
            }
        }

        [HttpGet("leagues/{leagueName}/teams")]
        public async Task<ActionResult<IEnumerable<Match>>> GetTeamsForLeague(string leagueName, int? season)
        {
            try
            {
                var result = await leagueRepository.GetTeamsForLeague(leagueName, season);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query teams data");
            }
        }

        [HttpGet("leagues/{leagueName}/rounds")]
        public async Task<ActionResult<IEnumerable<Match>>> GetRoundsForLeagueAndSeason(string leagueName, int? season)
        {
            if (season == null)
                return BadRequest("Season has to be specified as query parameter to retrieve rounds");

            try
            {
                var result = await leagueRepository.GetRoundsForLeagueAndSeason(leagueName, (int)season);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query rounds data");
            }
        }

        [HttpGet("leagues/{leagueName}/matches")]
        public async Task<ActionResult<IEnumerable<Match>>> GetMatchesForLeague(string leagueName, int? season, string? round)
        {
            try
            {
                var result = await leagueRepository.GetMatchesForLeague(leagueName, season, round);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query seasons data");
            }
        }

        [HttpGet("leagues/{leagueName}/standings")]
        public async Task<ActionResult<IEnumerable<Match>>> GetStandingsForLeagueAndSeason(string leagueName, int? season)
        {
            if (season == null)
                return BadRequest("Season has to be specified as query parameter to retrieve standings");

            try
            {
                var result = await leagueRepository.GetStandingsForLeagueAndSeason(leagueName, (int)season);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query seasons data");
            }
        }

        [HttpGet("leagues/{leagueName}/topscorers")]
        public async Task<ActionResult<IEnumerable<Match>>> GetTopScorersForLeagueAndSeason(string leagueName, int? season)
        {
            if (season == null)
                return BadRequest("Season has to be specified as query parameter to retrieve top scorers");
            try
            {
                var result = await leagueRepository.GetTopScorersForLeagueAndSeason(leagueName, (int)season);
                return Ok(result);
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
                return Ok(result);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query leagues data");
            }
        }
    }
}
