using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class SquadPlayer
    {
        [JsonProperty("id")]
        public int? ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("age")]
        public int? Age { get; set; }

        [JsonProperty("number")]
        public int? Number { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("photo")]
        public string Photo { get; set; }
    }
}
