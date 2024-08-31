using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class TeamsResponseItem
    {
        [JsonProperty("team")]
        public Team Team { get; set; }

        [JsonProperty("venue")]
        public TeamVenue Venue { get; set; }
    }
}