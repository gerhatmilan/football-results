using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class SquadsResponseItem
    {
        [JsonProperty("team")]
        public SquadTeam Team { get; set; }

        [JsonProperty("players")]
        public IEnumerable<SquadPlayer> Players { get; set; }
    }
}
