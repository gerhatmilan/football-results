using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class TeamVenue
    {
        [JsonProperty("id")]
        public int? ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("capacity")]
        public int? Capacity { get; set; }

        [JsonProperty("surface")]
        public string Surface { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }
    }
}
