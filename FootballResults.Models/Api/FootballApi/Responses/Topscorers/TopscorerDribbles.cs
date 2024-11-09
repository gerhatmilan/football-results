using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class TopscorerDribbles
    {
        [JsonProperty("attempts")]
        public int? Attempts { get; set; }

        [JsonProperty("success")]
        public int? Success { get; set; }

        [JsonProperty("past")]
        public int? Past { get; set; }
    }
}
