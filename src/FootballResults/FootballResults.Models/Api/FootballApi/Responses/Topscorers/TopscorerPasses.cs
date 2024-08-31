using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class TopscorerPasses
    {
        [JsonProperty("total")]
        public int? Total { get; set; }

        [JsonProperty("key")]
        public int? Key { get; set; }

        [JsonProperty("accuracy")]
        public int? Accuracy { get; set; }
    }
}
