using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class StandingLeague
    {
        [JsonProperty("id")]
        public int? ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("flag")]
        public string Flag { get; set; }

        [JsonProperty("season")]
        public int? Season { get; set; }

        [JsonProperty("standings")]
        public IEnumerable<IEnumerable<Standing>> Standings { get; set; }
    }
}
