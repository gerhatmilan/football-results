namespace FootballResults.WebApp.Services.Time
{
    public interface IClientTimeService
    {
        Task<TimeSpan> GetClientUtcDiffAsync();

        Task<DateTime> GetClientDateAsync();
    }
}
