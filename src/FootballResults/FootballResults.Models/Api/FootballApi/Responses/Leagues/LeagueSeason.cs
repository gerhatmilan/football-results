using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class LeagueSeason
    {
        [JsonProperty("year")]
        public int? Year { get; set; }

        [JsonProperty("start")]
        public DateTime? Start { get; set; }

        [JsonProperty("end")]
        public DateTime? End { get; set; }

        [JsonProperty("current")]
        public bool? Current { get; set; }

        [JsonProperty("coverage")]
        public LeagueSeasonCoverage Coverage { get; set; }
    }
}
