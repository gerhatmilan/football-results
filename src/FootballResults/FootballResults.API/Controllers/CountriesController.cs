using FootballResults.API.Models;
using FootballResults.Models;
using Microsoft.AspNetCore.Mvc;

namespace FootballResults.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryRepository countryRepository;

        public CountriesController(ICountryRepository countryRepository)
        {
            this.countryRepository = countryRepository;
        }

        [HttpGet("countries")]
        public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
        {
            try
            {
                var result = await countryRepository.GetCountries();
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query countries data");
            }
        }

        [HttpGet("countries/leagues")]
        public async Task<ActionResult<IEnumerable<Country>>> GetLeaguesByCountry()
        {
            try
            {
                var result = await countryRepository.GetLeaguesByCountry();
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query leagues data");
            }
        }

        [HttpGet("countries/teams")]
        public async Task<ActionResult<IEnumerable<Country>>> GetTeamsByCountry()
        {
            try
            {
                var result = await countryRepository.GetTeamsByCountry();
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query teams data");
            }
        }

        [HttpGet("countries/venues")]
        public async Task<ActionResult<IEnumerable<Country>>> GetVenuesByCountry()
        {
            try
            {
                var result = await countryRepository.GetVenuesByCountry();
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query venues data");
            }
        }

        [HttpGet("countries/{countryName}")]
        public async Task<ActionResult<Country>> GetCountryByName(string countryName)
        {
            try
            {
                countryName = countryName.Replace("-", " ");

                var result = await countryRepository.GetCountryByName(countryName);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query country data");
            }
        }   

        [HttpGet("countries/{countryName}/leagues")]
        public async Task<ActionResult<IEnumerable<League>>> GetLeaguesInCountry(string countryName)
        {
            countryName = countryName.ToLower();
            var splitted = countryName.Split("-").ToList();
            countryName = String.Join(" ", splitted.Select(word => char.ToUpper(word[0]) + word.Substring(1)));

            try
            {
                var result = await countryRepository.GetLeaguesInCountry(countryName);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query leagues data");
            }
        }

        [HttpGet("countries/{countryName}/teams")]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeamsInCountry(string countryName)
        {
            countryName = countryName.ToLower();
            var splitted = countryName.Split("-").ToList();
            countryName = String.Join(" ", splitted.Select(word => char.ToUpper(word[0]) + word.Substring(1)));

            try
            {
                var result = await countryRepository.GetTeamsInCountry(countryName);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query teams data");
            }
        }

        [HttpGet("countries/{countryName}/venues")]
        public async Task<ActionResult<IEnumerable<Venue>>> GetVenuesInCountry(string countryName)
        {
            countryName = countryName.ToLower();
            var splitted = countryName.Split("-").ToList();
            countryName = String.Join(" ", splitted.Select(word => char.ToUpper(word[0]) + word.Substring(1)));

            try
            {
                var result = await countryRepository.GetVenuesInCountry(countryName);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query venues data");
            }
        }

        [HttpGet("countries/search")]
        public async Task<ActionResult<IEnumerable<Country>>> Search(string? countryName)
        {
            try
            {
                var result = await countryRepository.Search(countryName);
                if (result.Any())
                {
                    return Ok(result);
                }
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not query countries data");
            }
        }
    }
}
