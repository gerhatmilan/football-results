using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class FixtureGoals
    {
        [JsonProperty("home")]
        public int? Home { get; set; }

        [JsonProperty("away")]
        public int? Away { get; set; }
    }
}
