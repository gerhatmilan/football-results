using FootballResults.API.Models;
using FootballResults.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace FootballResults.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchRepository matchRepository;

        public MatchesController(IMatchRepository matchRepository)
        {
            this.matchRepository= matchRepository;
        }

        [HttpGet("matches/{id}/")]
        public async Task<ActionResult<Match?>> GetMatchByID(int id)
        {
            try
            {
                var result = await matchRepository.GetMatchByID(id);
                return result != null ? Ok(result) : NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query match data");
            }
        }

        [HttpGet("matches/head2head")]
        public async Task<ActionResult<IEnumerable<Match>>> GetHeadToHead(string? team1, string? team2)
        {
            if (team1 == null || team2 == null)
                return BadRequest("Two teams has to be specified as query parameters to get head to head matches");

            try
            {
                var result = await matchRepository.GetHeadToHead(team1, team2);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query head to head data");
            }
        }

        [HttpGet("matches/search")]
        public async Task<ActionResult<IEnumerable<Match>>> Search(DateTime? date, int? year, int? month, int? day, string? team, string? league, int? season, string? round)
        {
            try
            {
                var result = await matchRepository.Search(date, year, month, day, team, league, season, round);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query matches data");
            }
        }
    }
}
