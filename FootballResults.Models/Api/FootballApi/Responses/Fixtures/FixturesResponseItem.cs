using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class FixturesResponseItem
    {
        [JsonProperty("fixture")]
        public Fixture Fixture { get; set; }

        [JsonProperty("league")]
        public FixtureLeague League { get; set; }

        [JsonProperty("teams")]
        public FixtureTeams Teams { get; set; }

        [JsonProperty("goals")]
        public FixtureGoals Goals { get; set; }

        [JsonProperty("score")]
        public FixtureScore Score { get; set; }
    }
}