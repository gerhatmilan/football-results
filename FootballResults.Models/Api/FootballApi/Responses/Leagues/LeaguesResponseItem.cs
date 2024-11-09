using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class LeaguesResponseItem
    {
        [JsonProperty("league")]
        public League League { get; set; }

        [JsonProperty("country")]
        public LeagueCountry Country { get; set; }

        [JsonProperty("seasons")]
        public IEnumerable<LeagueSeason> Seasons { get; set; }
    }
}
