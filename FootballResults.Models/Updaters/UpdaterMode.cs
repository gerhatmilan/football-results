namespace FootballResults.Models.Updaters
{
    public enum UpdaterMode
    {
        Classic,
        Helper,
        AllLeaguesAllSeasons,
        AllLeaguesCurrentSeason,
        AllLeaguesSpecificSeason,
        ActiveLeaguesAllSeasons,
        ActiveLeaguesCurrentSeason,
        ActiveLeaguesSpecificSeason,
        SpecificLeagueAllSeasons,
        SpecificLeagueCurrentSeason,
        CurrentDate,
        CurrentDateActiveLeagues,
        SpecificDate,
        SpecificDateActiveLeagues,
        SpecificTeam,
        SpecificCountryAllTeams,
        BasedOnLastUpdate
    }
}
