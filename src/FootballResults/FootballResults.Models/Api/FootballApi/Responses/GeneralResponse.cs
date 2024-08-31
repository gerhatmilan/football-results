using FootballResults.Models.Api.FootballApi.Responses.CustomConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class GeneralResponse<TResponse>
    {
        [JsonProperty("get")]
        public string Get { get; set; }

        [JsonProperty("parameters")]
        [JsonConverter(typeof(KeyValuePairConverter<string>))]
        public Dictionary<string, string> Parameters { get; set; }

        [JsonProperty("errors")]
        [JsonConverter(typeof(KeyValuePairConverter<string>))]
        public Dictionary<string, string> Errors { get; set; }

        [JsonProperty("results")]
        public int Results { get; set; }

        [JsonProperty("paging")]
        public Paging Paging { get; set; }

        [JsonProperty("response")]
        public virtual IEnumerable<TResponse> Response { get; set; }
    }
}
