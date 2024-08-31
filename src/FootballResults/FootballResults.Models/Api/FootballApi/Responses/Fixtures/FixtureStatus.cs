using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class FixtureStatus
    {
        [JsonProperty("long")]
        public string Long { get; set; }

        [JsonProperty("short")]
        public string Short { get; set; }

        [JsonProperty("elapsed")]
        public int? Elapsed { get; set; }
    }
}
