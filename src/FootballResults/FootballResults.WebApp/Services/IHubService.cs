namespace FootballResults.WebApp.Services
{
    public interface IHubService : IAsyncDisposable
    {
        public bool IsConnected { get; }

        public Task StartAsync();
        public Task StopAsync();
    }
}
