using FootballResults.DataAccess.Entities.Football;
using FootballResults.Models.Api.FootballApi;
using FootballResults.Models.Api.FootballApi.Responses;
using Microsoft.Extensions.Options;

namespace FootballResults.DatabaseUpdaters.Updaters
{
    [Updater]
    [SupportedModes(UpdaterMode.Classic)]
    public class CountryUpdater : Updater<CountriesResponse, CountriesResponseItem>
    {
        protected override UpdaterSpecificSettings UpdaterSpecificSettings => _apiConfig.DataFetch.Countries;

        public CountryUpdater(IServiceScopeFactory serviceScopeFactory, ILogger<CountryUpdater> logger, IOptions<FootballApiConfig> apiConfig)
            : base(serviceScopeFactory, logger, apiConfig) { }

        protected override void ProcessData(IEnumerable<CountriesResponseItem> responseItems)
        {
            List<Country> existingCountries = _dbContext.Countries.ToList();

            foreach (Country? responseRecord in responseItems.Select(MapCountry))
            {
                if (responseRecord == null)
                {
                    _logger.LogWarning("Invalid country, skipping...");
                    continue;
                }

                Country? existingRecord = existingCountries.FirstOrDefault(existingCountry => existingCountry.Name.Equals(responseRecord.Name));

                if (existingRecord == null)
                {
                    existingCountries.Add(responseRecord);
                    Add(responseRecord);
                }
                else if (!existingRecord.Equals(responseRecord))
                    Update(existingRecord, responseRecord);

                _dbContext.SaveChanges();
            }
        }

        private void Add(Country responseRecord)
        {
            _logger.LogDebug($"Adding new country: {responseRecord.Name}");
            _dbContext.Countries.Add(responseRecord);
        }

        private void Update(Country existingRecord, Country responseRecord)
        {
            _logger.LogDebug($"Updating country with ID {existingRecord.ID}:"
                + (existingRecord.Name != responseRecord.Name ? $"\n\tName: {existingRecord.Name} -> {responseRecord.Name}" : "")
                + (existingRecord.FlagLink != responseRecord.FlagLink ? $"\n\tFlagLink: {existingRecord.FlagLink} -> {responseRecord.FlagLink}" : ""));

            existingRecord.CopyFrom(responseRecord);
        }

        public static bool ValidateCountry(CountriesResponseItem responseItem)
        {
            return !string.IsNullOrWhiteSpace(responseItem.Name);
        }

        public static Country? MapCountry(CountriesResponseItem responseItem)
        {
            if (ValidateCountry(responseItem))
            {
                return new Country
                {
                    Name = responseItem.Name,
                    FlagLink = responseItem.Flag
                };
            }
            else
            {
                return null;
            }
        }
    }
}