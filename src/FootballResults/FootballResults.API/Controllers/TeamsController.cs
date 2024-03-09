using FootballResults.API.Models;
using FootballResults.Models;
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
                var result = await teamRepository.GetTeams();
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
                teamName = teamName.Replace("-", " ");

                var result = await teamRepository.GetTeamByName(teamName);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query team data");
            }
        }

        [HttpGet("teams/search")]
        public async Task<ActionResult<IEnumerable<Team>>> Search(string? teamName, string? country, bool? national)
        {
            try
            {
                var result = await teamRepository.Search(teamName, country, national);
                if (result.Any())
                {
                    return Ok(result);
                }
                return NotFound();
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query teams data");
            }
        }
    }
}
