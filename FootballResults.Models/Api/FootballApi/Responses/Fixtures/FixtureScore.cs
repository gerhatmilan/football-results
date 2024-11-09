using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class FixtureScore
    {
        [JsonProperty("halftime")]
        public FixtureScoreDetail Halftime { get; set; }

        [JsonProperty("fulltime")]
        public FixtureScoreDetail Fulltime { get; set; }

        [JsonProperty("extratime")]
        public FixtureScoreDetail Extratime { get; set; }

        [JsonProperty("penalty")]
        public FixtureScoreDetail Penalty { get; set; }
    }
}
