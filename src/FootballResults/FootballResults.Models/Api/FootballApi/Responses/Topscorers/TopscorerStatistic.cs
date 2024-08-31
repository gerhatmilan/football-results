using Newtonsoft.Json;

namespace FootballResults.Models.Api.FootballApi.Responses
{
    public class TopscorerStatistic
    {
        [JsonProperty("team")]
        public TopscorerTeam Team { get; set; }

        [JsonProperty("league")]
        public TopscorerLeague League { get; set; }

        [JsonProperty("games")]
        public TopscorerGames Games { get; set; }

        [JsonProperty("substitutes")]
        public TopscorerSubstitutes Substitutes { get; set; }

        [JsonProperty("shots")]
        public TopscorerShots Shots { get; set; }

        [JsonProperty("goals")]
        public TopscorerGoals Goals { get; set; }

        [JsonProperty("passes")]
        public TopscorerPasses Passes { get; set; }

        [JsonProperty("tackles")]
        public TopscorerTackles Tackles { get; set; }

        [JsonProperty("duels")]
        public TopscorerDuels Duels { get; set; }

        [JsonProperty("dribbles")]
        public TopscorerDribbles Dribbles { get; set; }

        [JsonProperty("fouls")]
        public TopscorerFouls Fouls { get; set; }

        [JsonProperty("cards")]
        public TopscorerCards Cards { get; set; }

        [JsonProperty("penalty")]
        public TopscorerPenalty Penalty { get; set; }
    }
}
