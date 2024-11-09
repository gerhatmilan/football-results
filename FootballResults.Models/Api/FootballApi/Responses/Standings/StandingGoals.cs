using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class StandingGoals
    {
        [JsonProperty("for")]
        public int? For { get; set; }

        [JsonProperty("against")]
        public int? Against { get; set; }
    }
}
