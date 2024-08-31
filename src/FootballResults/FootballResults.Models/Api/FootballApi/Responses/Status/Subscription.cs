using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class Subscription
    {
        [JsonProperty("plan")]
        public string Plan { get; set; }

        [JsonProperty("end")]
        public DateTimeOffset? End { get; set; }

        [JsonProperty("active")]
        public bool? Active { get; set; }
    }
}