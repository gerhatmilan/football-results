using FootballResults.DataAccess.Entities.Football;
using FootballResults.Models.Api.FootballApi.Responses;
using FootballResults.Models.Config;
using Microsoft.Extensions.Options;

namespace FootballResults.DatabaseUpdaters.Updaters
{
    [Updater]
    [SupportedModes(UpdaterMode.Helper)]
    public class VenueUpdaterForMatch : Updater<FixturesResponse, FixturesResponseItem>
    {
        protected override UpdaterSpecificSettings UpdaterSpecificSettingsForLeagueAndSeason => _apiConfig.DataFetch.MatchesForLeagueAndSeason;

        public VenueUpdaterForMatch(IServiceScopeFactory serviceScopeFactory, ILogger<VenueUpdaterForMatch> logger)
            : base(serviceScopeFactory, logger) { }

        protected override void ProcessData(IEnumerable<FixturesResponseItem> responseItems)
        {
            var existingVenues = _dbContext.Venues.ToList();

            for (int i = 0; i < responseItems.Count(); i++)
            {
                var responseItem = responseItems.ElementAt(i);
                var mappedVenue = MapVenue(responseItem.Fixture.Venue);

                Country? relatedCountry = _dbContext.Countries.FirstOrDefault(country => country.Name.Equals(responseItem.League.Country));

                if (mappedVenue == null)
                {
                    continue;
                }
                else if (relatedCountry == null)
                {
                    _logger.LogWarning($"Country does not exist in the database for venue {mappedVenue.Name}, skipping...");
                    continue;
                }

                mappedVenue.CountryID = relatedCountry.ID;
                Venue? existingRecord = existingVenues.FirstOrDefault(existingVenues => existingVenues.ID == mappedVenue.ID);

                if (existingRecord == null)
                {
                    existingVenues.Add(mappedVenue);
                    Add(mappedVenue);
                }
                // we dont update venues at this point
            }

            _dbContext.SaveChanges();
        }

        private void Add(Venue newVenue)
        {
            _logger.LogDebug($"Adding new venue: {newVenue.Name}");
            _dbContext.Venues.Add(newVenue);
        }

        private void Update(Venue existingRecord, Venue responseRecord)
        {
            _logger.LogDebug($"Updating venue with ID {existingRecord.ID}:"
                + (existingRecord.ID != responseRecord.ID ? $"\n\tID: {existingRecord.ID} -> {responseRecord.ID}" : "")
                + (existingRecord.City != responseRecord.City ? $"\n\tCity: {existingRecord.City} -> {responseRecord.City}" : "")
                + (existingRecord.Name != responseRecord.Name ? $"\n\tName: {existingRecord.Name} -> {responseRecord.Name}" : ""));

            existingRecord.CopyFrom(responseRecord);
        }

        public static bool ValidateVenue(FixtureVenue responseItem)
        {
            return responseItem.ID.HasValue;
        }

        public static Venue? MapVenue(FixtureVenue responseItem)
        {
            if (ValidateVenue(responseItem))
            {
                return new Venue
                {
                    ID = responseItem.ID!.Value,
                    City = responseItem.City,
                    Name = responseItem.Name
                };
            }
            else
            {
                return null;
            }
        }
    }
}
