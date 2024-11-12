using Extensions;
using System.Reflection;

namespace FootballResults.Models.Updaters.Services
{
    public class UpdaterRunnerService : IUpdaterRunnerService
    {
        private IUpdater _selectedUpdater;
        private UpdaterMode? _selectedMode;
        private UpdaterMenuMode _menuMode;
        private IServiceProvider _serviceProvider;

        public IUpdater SelectedUpdater
        {
            get => _selectedUpdater;
            private set
            {
                _selectedUpdater = value;
                _menuMode = UpdaterMenuMode.ShowModes;
            }
        }
        public UpdaterMode? SelectedMode { get => _selectedMode; set => _selectedMode = value; }
        public UpdaterMenuMode MenuMode
        {
            get => _menuMode;
            set
            {
                if (_menuMode == UpdaterMenuMode.ShowModes && value == UpdaterMenuMode.ShowUpdaters)
                {
                    _selectedMode = null;
                }

                _menuMode = value;
            }
        }

        public int SelectedUpdaterIndex => _selectedUpdater != null
            ? IUpdaterRunnerService.AvailableUpdaters.ToList().IndexOf(_selectedUpdater.GetType())
            : -1;

        public int SelectedModeIndex => _selectedUpdater != null && _selectedMode != null
            ? _selectedUpdater.SupportedModes.ToList().IndexOf(_selectedMode.Value)
            : -1;

        public UpdaterRunnerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _selectedUpdater = null;
            _selectedMode = null;
            _menuMode = UpdaterMenuMode.ShowUpdaters;
        }

        public async Task RunUpdaterAsync(params object[] modeParameters)
        {
            await _selectedUpdater!.StartAsync(_selectedMode!.Value, modeParameters);
            _menuMode = UpdaterMenuMode.ShowUpdaters;
        }

        public void SetUpdater(string updaterName)
        {
            Type updaterType = IUpdaterRunnerService.AvailableUpdaters.FirstOrDefault(i => i.Name == updaterName);

            if (updaterType == null)
            {
                throw new ArgumentException("Invalid updater name");
            }

            SelectedUpdater = IUpdaterRunnerService.CreateUpdater(updaterType, _serviceProvider);
        }

        public void SetUpdater(int index)
        {
            Type updaterType = IUpdaterRunnerService.AvailableUpdaters.ElementAt(index);

            if (updaterType == null)
            {
                throw new ArgumentException("Invalid updater name");
            }

            SelectedUpdater = IUpdaterRunnerService.CreateUpdater(updaterType, _serviceProvider);
        }

        public void SetUpdaterMode(int index)
        {
            if (_selectedUpdater == null)
            {
                throw new InvalidOperationException();
            }

            _selectedMode = _selectedUpdater.SupportedModes.ElementAt(index);
        }

        public void Reset()
        {
            _selectedUpdater = null;
            _selectedMode = null;
            _menuMode = UpdaterMenuMode.ShowUpdaters; ;
        }
    }
}