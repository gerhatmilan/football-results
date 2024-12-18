﻿namespace FootballResults.WebApp.Components.Utilities
{
    public class MatchFilterParameters
    {
        public int? YearFilter { get; set; }
        public int? MonthFilter { get; set; }
        public int? DayFilter { get; set; }
        public string? TeamFilter { get; set; }
        public string? OpponentNameFilter { get; set; }
        public string? LeagueFilter { get; set; }
        public int? SeasonFilter { get; set; }
        public string? RoundFilter { get; set; }
        public string? HomeAwayFilter { get; set; } // ["All", "Home", "Away"]
    }
}
