using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class TopscorerDuels
    {
        [JsonProperty("total")]
        public int? Total { get; set; }

        [JsonProperty("won")]
        public int? Won { get; set; }
    }
}
