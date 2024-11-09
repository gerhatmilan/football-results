using FootballResults.DataAccess.Repositories.Football;
using FootballResults.DataAccess.Entities.Football;
using Microsoft.AspNetCore.Mvc;

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
                var result = await matchRepository.GetByIDAsync(id, tracking: false);
                return Ok(result);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
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
        public async Task<ActionResult<IEnumerable<Match>>> Search(DateTime? date, DateTime? from, DateTime? to, int? year, int? month, int? day, string? team, string? league, int? season, string? round)
        {
            try
            {
                var result = await matchRepository.Search(date, from, to, year, month, day, team, league, season, round);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query matches data");
            }
        }
    }
}
