using Microsoft.EntityFrameworkCore;

namespace FootballResults.DataAccess.Entities.Football
{
    public class Country : Entity
    {
        /// <summary>
        /// Name of the country
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A link pointing to an image of the country's flag
        /// </summary>
        public string FlagLink { get; set; }

        /// <summary>
        /// Leagues in the country
        /// </summary>
        public IEnumerable<League> Leagues { get; set; }

        /// <summary>
        /// Teams in the country
        /// </summary>
        public IEnumerable<Team> Teams { get; set; }

        /// <summary>
        /// Venues in the country
        /// </summary>
        public IEnumerable<Venue> Venues { get; set; }

        public bool Equals(Country country)
        {
            return Name == country.Name && FlagLink == country.FlagLink;
        }

        public void CopyFrom(Country country)
        {
            Name = country.Name;
            FlagLink = country.FlagLink;
        }
    }
}