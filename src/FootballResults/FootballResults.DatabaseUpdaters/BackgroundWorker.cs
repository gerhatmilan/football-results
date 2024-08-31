namespace FootballResults.DatabaseUpdaters
{
    public class BackgroundWorker : BackgroundService
    {
        protected readonly IServiceScopeFactory _serviceScopeFactory;
        protected readonly ILogger _logger;

        public BackgroundWorker(IServiceScopeFactory serviceScopeFactory, ILogger logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
