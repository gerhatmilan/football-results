using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class TopscorerSubstitutes
    {
        [JsonProperty("in")]
        public int? In { get; set; }

        [JsonProperty("out")]
        public int? Out { get; set; }

        [JsonProperty("bench")]
        public int? Bench { get; set; }
    }
}
