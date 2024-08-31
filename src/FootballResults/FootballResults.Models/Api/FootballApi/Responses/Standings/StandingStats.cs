using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class StandingStats
    {
        [JsonProperty("played")]
        public int? Played { get; set; }

        [JsonProperty("win")]
        public int? Win { get; set; }

        [JsonProperty("draw")]
        public int? Draw { get; set; }

        [JsonProperty("lose")]
        public int? Lose { get; set; }

        [JsonProperty("goals")]
        public StandingGoals Goals { get; set; }
    }
}
