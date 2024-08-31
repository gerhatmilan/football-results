using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class TopscorerGames
    {
        [JsonProperty("appearences")]
        public int? Appearences { get; set; }

        [JsonProperty("lineups")]
        public int? Lineups { get; set; }

        [JsonProperty("minutes")]
        public int? Minutes { get; set; }

        [JsonProperty("number")]
        public int? Number { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("rating")]
        public string Rating { get; set; }

        [JsonProperty("captain")]
        public bool? Captain { get; set; }
    }
}