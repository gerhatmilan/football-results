using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class League
    {
        [JsonProperty("id")]
        public int? ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }
    }
}
