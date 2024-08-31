using FootballResults.DataAccess;
using FootballResults.DatabaseUpdaters.Updaters;
using FootballResults.Models.Api.FootballApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FootballResults.DatabaseUpdaters.UpdaterMenu
{
    public class UpdaterRunner : BackgroundService
    {
        private readonly FootballApiConfig _apiConfig;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger<UpdaterRunner> _logger;
        private readonly IHostEnvironment _hostingEnvironment;

        private bool _running = true;
        private UpdaterMenuHandler _menuHandler;
        private IUpdater? _selectedUpdater;
        private UpdaterMode? _selectedMode;
        private IEnumerable<IncludedLeagueRecord> IncludedLeagues => _apiConfig.IncludedLeagues;

        public UpdaterRunner(IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime, ILogger<UpdaterRunner> logger, IHostEnvironment hostingEnvironment)
        {
            _apiConfig = serviceProvider.GetRequiredService<IOptions<FootballApiConfig>>().Value;
            _serviceProvider = serviceProvider;
            _applicationLifetime = applicationLifetime;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _menuHandler = new UpdaterMenuHandler(IncludedLeagues);
        }

        public override Task StartAsync(CancellationToken stoppingToken)
        {
            _applicationLifetime.ApplicationStarted.Register(async () =>
            {
                await ExecuteAsync(stoppingToken);
            });

            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _menuHandler.ShowDescription();
            _menuHandler.SaveCursorPosition();

            while (!cancellationToken.IsCancellationRequested && _running)
            {
                _menuHandler.ShowMenu(_selectedUpdater, _selectedMode);
                _menuHandler.WaitForInput();

                try
                {
                    await ProcessInputAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    if (_hostingEnvironment.IsDevelopment())
                    {
                        _logger.LogError(e.ToString());
                    }
                    else
                    {
                        _logger.LogError(e.Message);
                    }

                    _menuHandler.ResetConsole(waitForInput: true);
                }
            }
        }

        private async Task ProcessInputAsync(CancellationToken cancellationToken)
        {
            switch (_menuHandler.SelectedKey.Key)
            {
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    OnArrowDown();
                    break;
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    OnArrowUp();
                    break;
                case ConsoleKey.Enter:
                    await OnEnterAsync();
                    break;
                case ConsoleKey.Q:
                    OnQuit();
                    break;
                case ConsoleKey.Backspace:
                    OnBackSpace();
                    break;
                default:
                    _menuHandler.ResetCursorPosition();
                    break;
            }
        }

        private void OnArrowDown()
        {
            switch (_menuHandler.MenuMode)
            {
                case UpdaterMenuMode.ShowUpdaters:
                    _menuHandler.SelectedOption = _menuHandler.SelectedOption == UpdaterFactory.AvailableUpdaters.Count() - 1 ? 0 : _menuHandler.SelectedOption + 1;
                    break;
                case UpdaterMenuMode.ShowModes:
                    _menuHandler.SelectedOption = _menuHandler.SelectedOption == _selectedUpdater!.SupportedModes.Count() - 1 ? 0 : _menuHandler.SelectedOption + 1;
                    break;
                case UpdaterMenuMode.ShowLeagues:
                    _menuHandler.SelectedOption = _menuHandler.SelectedOption == IncludedLeagues.Count() - 1 ? 0 : _menuHandler.SelectedOption + 1;
                    break;
            }
            
            _menuHandler.ResetCursorPosition();
        }

        private void OnArrowUp()
        {
            switch (_menuHandler.MenuMode)
            {
                case UpdaterMenuMode.ShowUpdaters:
                    _menuHandler.SelectedOption = _menuHandler.SelectedOption == 0 ? UpdaterFactory.AvailableUpdaters.Count() - 1 : _menuHandler.SelectedOption - 1;
                    break;
                case UpdaterMenuMode.ShowModes:
                    _menuHandler.SelectedOption = _menuHandler.SelectedOption == 0 ? _selectedUpdater!.SupportedModes.Count() - 1 : _menuHandler.SelectedOption - 1;
                    break;
                case UpdaterMenuMode.ShowLeagues:
                    _menuHandler.SelectedOption = _menuHandler.SelectedOption == 0 ? IncludedLeagues.Count() - 1 : _menuHandler.SelectedOption - 1;
                    break;
            }
            _menuHandler.ResetCursorPosition();
        }

        private async Task OnEnterAsync()
        {
            switch (_menuHandler.MenuMode)
            {
                case UpdaterMenuMode.ShowUpdaters:
                    OnUpdaterSelected();
                    break;
                case UpdaterMenuMode.ShowModes:
                    await OnModeSelectedAsync();
                    break;
                case UpdaterMenuMode.ShowLeagues:
                    await OnLeagueSelectedAsync();
                    break;
            }
        }

        private void OnQuit()
        {
            _applicationLifetime.StopApplication();
            _running = false;
        }

        private void OnBackSpace()
        {
            if (_menuHandler.MenuMode == UpdaterMenuMode.ShowModes)
            {
                _menuHandler.MenuMode = UpdaterMenuMode.ShowUpdaters;
                _menuHandler.SelectedOption = UpdaterFactory.AvailableUpdaters.ToList().IndexOf(_selectedUpdater!.GetType());
                _menuHandler.ResetConsole();
            }
            else if (_menuHandler.MenuMode == UpdaterMenuMode.ShowLeagues)
            {
                _menuHandler.MenuMode = UpdaterMenuMode.ShowModes;
                _menuHandler.SelectedOption = _selectedUpdater!.SupportedModes.ToList().IndexOf(_selectedMode!.Value);
                _menuHandler.ResetConsole();
            }
            else
            {
                _menuHandler.ResetCursorPosition();
            }
        }

        private async Task RunUpdaterAsync(params object[] modeParameters)
        {
            await _selectedUpdater!.StartAsync(_selectedMode!.Value, modeParameters);
            _menuHandler.MenuMode = UpdaterMenuMode.ShowUpdaters;
            _menuHandler.ResetConsole(waitForInput: true);
        }

        private void OnUpdaterSelected()
        {
            _selectedUpdater = UpdaterFactory.CreateUpdater(_menuHandler.SelectedOption, _serviceProvider);
            _menuHandler.MenuMode = UpdaterMenuMode.ShowModes;
            _menuHandler.ResetConsole();
        }

        private async Task OnModeSelectedAsync()
        {
            _selectedMode = _selectedUpdater?.SupportedModes?.ElementAt(_menuHandler.SelectedOption);
            switch (_selectedMode)
            {
                case UpdaterMode.SpecificDate:
                    await RunUpdaterAsync(_menuHandler.GetDateFromUser());
                    break;
                case UpdaterMode.SpecificLeagueCurrentSeason:
                    _menuHandler.MenuMode = UpdaterMenuMode.ShowLeagues;
                    _menuHandler.ResetConsole();
                    break;
                case UpdaterMode.SpecificTeam:
                    await RunUpdaterAsync(_menuHandler.GetTeamFromUser());
                    break;
                case UpdaterMode.BasedOnLastUpdate:
                    await RunUpdaterAsync(_menuHandler.GetLastUpdateBoundaryFromUser());
                    break;
                default:
                    await RunUpdaterAsync();
                    break;
            }
        }

        private async Task OnLeagueSelectedAsync()
        {
            await RunUpdaterAsync(IncludedLeagues.ElementAt(_menuHandler.SelectedOption).ID);
        }
    }
}
