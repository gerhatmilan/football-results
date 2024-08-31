using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class Paging
    {
        [JsonProperty("current")]
        public int Current { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }
}
