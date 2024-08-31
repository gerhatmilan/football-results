using FootballResults.Models.Api.FootballApi.Responses;
using FootballResults.Models.Api.FootballApi.Responses.CustomConverters;
using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class StatusResponse : GeneralResponse<object>
    {
        [JsonProperty("response")]
        [JsonConverter(typeof(ResponseConverter<StatusResponseItem>))]
        new public object Response { get; set; }
    }
}
