namespace FootballResults.Models
{
    public class Country
    {
        public string CountryID { get; set; }

        public string FlagLink { get; set; }

        public ICollection<League> Leagues { get; set; }

        public ICollection<Team> Teams { get; set; }

        public ICollection<Venue> Venues { get; set; }

    }
}