using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class StandingTeam
    {
        [JsonProperty("id")]
        public int? ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("winner")]
        public bool? Winner { get; set; }
    }
}
