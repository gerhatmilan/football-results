namespace FootballResults.WebApp.Components.Other
{
    public class MatchFilterParameters
    {
        // Match filters
        public int? YearFilter { get; set; }
        public int? MonthFilter { get; set; }
        public int? DayFilter { get; set; }
        public string? TeamFilter { get; set; }
        public  string? LeagueFilter { get; set; }
        public int? SeasonFilter { get; set; }
        public string? RoundFilter { get; set; }
        // ["All", "Home", "Away"]
        public string? HomeAwayFilter { get; set; }
    }
}
