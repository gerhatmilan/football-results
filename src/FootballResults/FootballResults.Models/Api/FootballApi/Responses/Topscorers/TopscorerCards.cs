using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class TopscorerCards
    {
        [JsonProperty("yellow")]
        public int? Yellow { get; set; }

        [JsonProperty("yellowred")]
        public int? Yellowred { get; set; }

        [JsonProperty("red")]
        public int? Red { get; set; }
    }
}
