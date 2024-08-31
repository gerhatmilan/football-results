using FootballResults.DataAccess.Repositories.Football;
using FootballResults.DataAccess.Entities.Football;
using Microsoft.AspNetCore.Mvc;

namespace FootballResults.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamRepository teamRepository;

        public TeamsController(ITeamRepository teamRepository)
        {
            this.teamRepository = teamRepository;
        }

        [HttpGet("teams")]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            try
            {
                var result = await teamRepository.GetAllAsync(tracking: false);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query teams data");
            }
        } 

        [HttpGet("teams/{teamName}")]
        public async Task<ActionResult<Team>> GetTeamByName(string teamName)
        {
            try
            {
                var result = await teamRepository.GetTeamByName(teamName);
                return result != null ? Ok(result) : NotFound(); 
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query team data");
            }
        }

        [HttpGet("teams/{teamName}/squad")]
        public async Task<ActionResult<IEnumerable<Player>>> GetSquadForTeam(string teamName)
        {
            try
            {
                var result = await teamRepository.GetSquadForTeam(teamName);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query squad data");
            }
        }

        [HttpGet("teams/{teamName}/matches")]
        public async Task<ActionResult<IEnumerable<Player>>> GetMatchesForTeam(string teamName, string? league, int? season)
        {
            if (league == null || season == null)
                return BadRequest("League name and season has to be specified as query parameters to retrieve match data");

            try
            {
                var result = await teamRepository.GetMatchesForTeamAndLeagueAndSeason(teamName, league, (int)season);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query matches data");
            }
        }

        [HttpGet("teams/search")]
        public async Task<ActionResult<IEnumerable<Team>>> Search(string? name, string? country, bool? national)
        {
            try
            {
                var result = await teamRepository.Search(name, country, national);
                return Ok(result);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query teams data");
            }
        }
    }
}
