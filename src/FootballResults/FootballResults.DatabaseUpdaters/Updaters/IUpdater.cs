namespace FootballResults.DatabaseUpdaters.Updaters
{
    public interface IUpdater
    {
        public IEnumerable<UpdaterMode> SupportedModes { get; }

        public Task StartAsync();
        public Task StartAsync(UpdaterMode mode, params object[] modeParameters);
    }
}
