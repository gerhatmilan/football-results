using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class Account
    {
        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("lastname")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}