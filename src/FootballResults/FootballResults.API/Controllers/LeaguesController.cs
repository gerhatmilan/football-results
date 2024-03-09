using FootballResults.API.Models;
using FootballResults.Models;
using Microsoft.AspNetCore.Mvc;

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

                if (result.Any())
                    return Ok(result);
                else
                    return NotFound();
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
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
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

                if (result.Any())
                    return Ok(result);
                else
                    return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query seasons data");
            }
        }


        [HttpGet("leagues/{leagueName}/{season}/rounds")]
        public async Task<ActionResult<IEnumerable<Match>>> GetRoundsForLeagueAndSeason(string leagueName, int season)
        {
            try
            {
                leagueName = leagueName.Replace("-", " ");

                var result = await leagueRepository.GetRoundsForLeagueAndSeason(leagueName, season);

                if (result.Any())
                    return Ok(result);
                else
                    return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query rounds data");
            }
        }

        [HttpGet("leagues/{leagueName}/{season}/matches/{round}")]
        public async Task<ActionResult<IEnumerable<Match>>> GetMatchesForLeagueAndSeason(string leagueName, int season, string round)
        {
            try
            {
                leagueName = leagueName.Replace("-", " ");

                var result = await leagueRepository.GetMatchesForLeagueAndSeasonAndRound(leagueName, season, round);

                if (result.Any())
                    return Ok(result);
                else
                    return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query seasons data");
            }
        }

        [HttpGet("leagues/{leagueName}/{season}/standings")]
        public async Task<ActionResult<IEnumerable<Match>>> GetStandingsForLeagueAndSeason(string leagueName, int season)
        {
            try
            {
                leagueName = leagueName.Replace("-", " ");

                var result = await leagueRepository.GetStandingsForLeagueAndSeason(leagueName, season);

                if (result.Any())
                    return Ok(result);
                else
                    return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query seasons data");
            }
        }

        [HttpGet("leagues/{leagueName}/{season}/topscorers")]
        public async Task<ActionResult<IEnumerable<Match>>> GetTopScorersForLeagueAndSeason(string leagueName, int season)
        {
            try
            {
                leagueName = leagueName.Replace("-", " ");

                var result = await leagueRepository.GetTopScorersForLeagueAndSeason(leagueName, season);
                
                if (result.Any())
                    return Ok(result);
                else
                    return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query seasons data");
            }
        }

        [HttpGet("leagues/search")]
        public async Task<ActionResult<IEnumerable<League>>> Search(string? leagueName, string? country, string? type, int? currentSeason)
        {
            try
            {
                var result = await leagueRepository.Search(leagueName, country, type, currentSeason);

                if (result.Any())
                    return Ok(result);
                else
                    return NotFound();
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query leagues data");
            }
        }
    }
}
