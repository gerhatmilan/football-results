using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class FixtureVenue
    {
        [JsonProperty("id")]
        public int? ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }
    }
}
