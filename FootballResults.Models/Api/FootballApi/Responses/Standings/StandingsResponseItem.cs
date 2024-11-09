using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class StandingsResponseItem
    {
        [JsonProperty("league")]
        public StandingLeague League { get; set; }
    }
}