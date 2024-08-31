using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class Requests
    {
        [JsonProperty("current")]
        public int? Current { get; set; }

        [JsonProperty("limit_day")]
        public int? LimitDay { get; set; }
    }
}