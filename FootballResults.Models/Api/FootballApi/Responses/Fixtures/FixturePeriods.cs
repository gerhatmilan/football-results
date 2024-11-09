using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class FixturePeriods
    {
        [JsonProperty("first")]
        public long? First { get; set; }

        [JsonProperty("second")]
        public long? Second { get; set; }
    }
}
