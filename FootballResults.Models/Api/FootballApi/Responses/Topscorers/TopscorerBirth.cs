using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class TopscorerBirth
    {
        [JsonProperty("date")]
        public DateTime? Date { get; set; }

        [JsonProperty("place")]
        public string Place { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }
    }
}
