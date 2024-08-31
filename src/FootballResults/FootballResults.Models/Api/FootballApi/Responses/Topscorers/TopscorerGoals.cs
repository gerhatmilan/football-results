using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class TopscorerGoals
    {
        [JsonProperty("total")]
        public int? Total { get; set; }

        [JsonProperty("conceded")]
        public int? Conceded { get; set; }

        [JsonProperty("assists")]
        public int? Assists { get; set; }

        [JsonProperty("saves")]
        public int? Saves { get; set; }
    }
}
