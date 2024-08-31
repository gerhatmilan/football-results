using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class StatusResponseItem
    {
        [JsonProperty("account")]
        public Account Account { get; set; }

        [JsonProperty("subscription")]
        public Subscription Subscription { get; set; }

        [JsonProperty("requests")]
        public Requests Requests { get; set; }
    }
}
