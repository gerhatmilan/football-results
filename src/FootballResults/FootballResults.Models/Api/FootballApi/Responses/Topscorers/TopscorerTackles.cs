using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class TopscorerTackles
    {
        [JsonProperty("total")]
        public int? Total { get; set; }

        [JsonProperty("blocks")]
        public int? Blocks { get; set; }

        [JsonProperty("interceptions")]
        public int? Interceptions { get; set; }
    }
}
