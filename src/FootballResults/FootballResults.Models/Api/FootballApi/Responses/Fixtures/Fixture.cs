using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class Fixture
    {
        [JsonProperty("id")]
        public int? ID { get; set; }

        [JsonProperty("referee")]
        public string Referee { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        [JsonProperty("date")]
        public DateTime? Date { get; set; }

        [JsonProperty("timestamp")]
        public long? Timestamp { get; set; }

        [JsonProperty("periods")]
        public FixturePeriods Periods { get; set; }

        [JsonProperty("venue")]
        public FixtureVenue Venue { get; set; }

        [JsonProperty("status")]
        public FixtureStatus Status { get; set; }
    }
}
