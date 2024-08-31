using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class CountriesResponseItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("flag")]
        public string Flag { get; set; }
    }
}
