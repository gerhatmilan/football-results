using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class TopscorerFouls
    {
        [JsonProperty("drawn")]
        public int? Drawn { get; set; }

        [JsonProperty("committed")]
        public int? Committed { get; set; }
    }
}
