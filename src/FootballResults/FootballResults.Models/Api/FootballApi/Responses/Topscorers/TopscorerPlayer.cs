using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class TopscorerPlayer
    {
        [JsonProperty("id")]
        public int? ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("firstname")]
        public string Firstname { get; set; }

        [JsonProperty("lastname")]
        public string Lastname { get; set; }

        [JsonProperty("age")]
        public int? Age { get; set; }

        [JsonProperty("birth")]
        public TopscorerBirth Birth { get; set; }

        [JsonProperty("nationality")]
        public string Nationality { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("weight")]
        public string Weight { get; set; }

        [JsonProperty("injured")]
        public bool? Injured { get; set; }

        [JsonProperty("photo")]
        public string Photo { get; set; }
    }
}
