using FootballResults.DataAccess.Entities.Football;
using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class TopscorersResponseItem
    {
        [JsonProperty("player")]
        public TopscorerPlayer Player { get; set; }

        [JsonProperty("statistics")]
        public IEnumerable<TopscorerStatistic> Statistics { get; set; }
    }
}