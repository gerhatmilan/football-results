using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class TopscorerShots
    {
        [JsonProperty("total")]
        public int? Total { get; set; }

        [JsonProperty("on")]
        public int? On { get; set; }
    }
}
