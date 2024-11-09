using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class FixtureTeams
    {
        [JsonProperty("home")]
        public FixtureTeam Home { get; set; }

        [JsonProperty("away")]
        public FixtureTeam Away { get; set; }
    }
}
