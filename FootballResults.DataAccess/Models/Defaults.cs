using FootballResults.DataAccess.Entities.Football;
using System.Collections.ObjectModel;

namespace FootballResults.DataAccess.Models
{
    public static class Defaults
    {
        public static IReadOnlyCollection<League> DefaultLeagues { get; } = new ReadOnlyCollection<League>
        (
            new List<League>()
            {
                new League { ID = 2,    Name=  "UEFA Champions League" },
                new League { ID = 4,    Name = "Euro Championship" },
                new League { ID = 39,   Name = "Premier League" },
                new League { ID = 61,   Name = "Ligue 1" },
                new League { ID = 78,   Name = "Bundesliga" },
                new League { ID = 135,  Name = "Serie A" },
                new League { ID = 140,  Name = "La Liga" }
            }
        );

        public static string FootballApiKeyEncryptionKey { get; } = "FootballApiKeyEncryptionKey";
        public static string BaseAddress { get; } = "https://v3.football.api-sports.io";
        public static string BaseAddressHeaderKey { get; } = "x-rapidapi-host";
        public static string ApiKeyHeaderKey { get; } = "x-rapidapi-key";
        public static int RateLimit { get; } = 10;
        public static string Countries { get; } = "Countries";
        public static string CountriesEndpoint { get; } = "/countries";
        public static string CountriesDefaultBackupPath { get; } = Path.Combine(".", "databackup", "countries", "countries.json");
        public static string Leagues { get; } = "Leagues";
        public static string LeaguesEndpint { get;  } = "/leagues";
        public static string LeaguesDefaultBackupPath { get; } = Path.Combine(".", "databackup", "leagues", "leagues.json");
        public static string TeamsForLeagueAndSeason { get; } = "TeamsForLeagueAndSeason";
        public static string TeamsForLeagueAndSeasonEndpoint { get; } = "/teams?league={0}&season={1}";
        public static string TeamsForLeagueAndSeasonDefaultBackupPath { get; } = Path.Combine(".", "databackup", "teams", "{0}", "{1}.json");
        public static string SquadForTeam { get; } = "SquadForTeam";
        public static string SquadForTeamEndpoint { get; } = "/players/squads?team={0}";
        public static string SquadForTeamDefaultBackupPath { get; } = Path.Combine(".", "databackup", "squads", "{0}.json");
        public static string MatchesForLeagueAndSeason { get; } = "MatchesForLeagueAndSeason";
        public static string MatchesForLeagueAndSeasonEndpoint { get; } = "/fixtures?league={0}&season={1}";
        public static string MatchesForLeagueAndSeasonDefaultBackupPath { get; } = Path.Combine(".", "databackup", "matches", "{0}", "{1}.json");
        public static string MatchesForDate { get; } = "MatchesForDate";
        public static string MatchesForDateEndpoint { get; } = "/fixtures?date={0}";
        public static string MatchesForDateDefaultBackupPath { get; } = Path.Combine(".", "databackup", "matches", "date", "{0}.json");
        public static string StandingsForLeagueAndSeason { get; } = "StandingsForLeagueAndSeason";
        public static string StandingsForLeagueAndSeasonEndpoint { get; } = "/standings?league={0}&season={1}";
        public static string StandingsForLeagueAndSeasonDefaultBackupPath { get; } = Path.Combine(".", "databackup", "standings", "{0}", "{1}.json");
        public static string TopScorersForLeagueAndSeason { get; } = "TopScorersForLeagueAndSeason";
        public static string TopScorersForLeagueAndSeasonEndpoint { get; } = "/players/topscorers?league={0}&season={1}";
        public static string TopScorersForLeagueAndSeasonDefaultBackupPath { get; } = Path.Combine(".", "databackup", "topscorers", "{0}", "{1}.json");
        public static string PredictionGamePicturesDirectory { get; } = Path.Combine("images", "prediction-games");
        public static string ProfilePicturesDirectory { get; } = Path.Combine("images", "profile-pictures");
        public static string CountriesDirectory { get; } = Path.Combine("images", "countries", "flags");
        public static string LeaguesDirectory { get; } = Path.Combine("images", "leagues", "logos");
        public static string TeamsDirectory { get; } = Path.Combine("images", "teams", "logos");
        public static string PlayersDirectory { get; } = Path.Combine("images", "players");
        public static string PredictionGameDefaultImage { get; } = Path.Combine("images", "prediction-games", "default.jpg");
    }
}
