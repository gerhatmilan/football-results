using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.Models.Updaters;
using FootballResults.Models.Updaters.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FootballResults.DatabaseUpdater
{
    public class UpdaterRunner : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger<UpdaterRunner> _logger;
        private readonly IHostEnvironment _hostingEnvironment;

        private bool _running = true;
        private UpdaterRunnerService _updaterRunnerService;
        private UpdaterMenuHandler _menuHandler;

        public UpdaterRunner(IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime, ILogger<UpdaterRunner> logger, IHostEnvironment hostingEnvironment)
        {
            _serviceProvider = serviceProvider;
            _applicationLifetime = applicationLifetime;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _updaterRunnerService = new UpdaterRunnerService(serviceProvider);
            _menuHandler = new UpdaterMenuHandler(_updaterRunnerService);
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
                _menuHandler.ShowMenu(_updaterRunnerService.SelectedUpdater, _updaterRunnerService.SelectedMode);
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
            switch (_updaterRunnerService.MenuMode)
            {
                case UpdaterMenuMode.ShowUpdaters:
                    _menuHandler.SelectedOption = _menuHandler.SelectedOption == IUpdaterRunnerService.AvailableUpdaters.Count() - 1 ? 0 : _menuHandler.SelectedOption + 1;
                    break;
                case UpdaterMenuMode.ShowModes:
                    _menuHandler.SelectedOption = _menuHandler.SelectedOption == _updaterRunnerService.SelectedUpdater!.SupportedModes.Count() - 1 ? 0 : _menuHandler.SelectedOption + 1;
                    break;
            }
            
            _menuHandler.ResetCursorPosition();
        }

        private void OnArrowUp()
        {
            switch (_updaterRunnerService.MenuMode)
            {
                case UpdaterMenuMode.ShowUpdaters:
                    _menuHandler.SelectedOption = _menuHandler.SelectedOption == 0 ? IUpdaterRunnerService.AvailableUpdaters.Count() - 1 : _menuHandler.SelectedOption - 1;
                    break;
                case UpdaterMenuMode.ShowModes:
                    _menuHandler.SelectedOption = _menuHandler.SelectedOption == 0 ? _updaterRunnerService.SelectedUpdater!.SupportedModes.Count() - 1 : _menuHandler.SelectedOption - 1;
                    break;
            }
            _menuHandler.ResetCursorPosition();
        }

        private async Task OnEnterAsync()
        {
            switch (_updaterRunnerService.MenuMode)
            {
                case UpdaterMenuMode.ShowUpdaters:
                    OnUpdaterSelected();
                    break;
                case UpdaterMenuMode.ShowModes:
                    await OnModeSelectedAsync();
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
            if (_updaterRunnerService.MenuMode == UpdaterMenuMode.ShowModes)
            {
                _updaterRunnerService.MenuMode = UpdaterMenuMode.ShowUpdaters;
                _menuHandler.ResetConsole();
            }
            else
            {
                _menuHandler.ResetCursorPosition();
            }
        }

        private async Task RunUpdaterAsync(params object[] modeParameters)
        {
            await _updaterRunnerService.RunUpdaterAsync(modeParameters);
            _menuHandler.ResetConsole(waitForInput: true);
        }

        private void OnUpdaterSelected()
        {
            _updaterRunnerService.SetUpdater(_menuHandler.SelectedOption);
            _menuHandler.ResetConsole();
        }

        private async Task OnModeSelectedAsync()
        {
            _updaterRunnerService.SetUpdaterMode(_menuHandler.SelectedOption);

            switch (_updaterRunnerService.SelectedMode)
            {
                case UpdaterMode.SpecificDate:
                case UpdaterMode.SpecificDateActiveLeagues:
                    DateTime? date = _menuHandler.GetDateFromUser();

                    if (date != null)
                    {
                        await RunUpdaterAsync(date);
                    }
                    else
                    {
                        _updaterRunnerService.MenuMode = UpdaterMenuMode.ShowModes;
                        _menuHandler.ResetConsole();
                    }
                    break;
                case UpdaterMode.SpecificLeagueAllSeasons:
                case UpdaterMode.SpecificLeagueCurrentSeason:
                case UpdaterMode.SpecificTeam:
                case UpdaterMode.SpecificCountryAllTeams:
                    int? id = _menuHandler.GetIDFromUser();

                    if (id != null)
                    {
                        await RunUpdaterAsync(id);
                    }
                    else
                    {
                        _updaterRunnerService.MenuMode = UpdaterMenuMode.ShowModes;
                        _menuHandler.ResetConsole();
                    }
                    break;
                case UpdaterMode.AllLeaguesSpecificSeason:
                case UpdaterMode.ActiveLeaguesSpecificSeason:
                    int? season = _menuHandler.GetSeasonFromUser();

                    if (season != null)
                    {
                        await RunUpdaterAsync(season);
                    }
                    else
                    {
                        _updaterRunnerService.MenuMode = UpdaterMenuMode.ShowModes;
                        _menuHandler.ResetConsole();
                    }
                    break;
                case UpdaterMode.BasedOnLastUpdate:
                    TimeSpan? lastUpdateBoundary = _menuHandler.GetLastUpdateBoundaryFromUser();

                    if (lastUpdateBoundary != null)
                    {
                        await RunUpdaterAsync(lastUpdateBoundary);
                    }
                    else
                    {
                        _updaterRunnerService.MenuMode = UpdaterMenuMode.ShowModes;
                        _menuHandler.ResetConsole();
                    }
                    break;
                default:
                    await RunUpdaterAsync();
                    break;
            }
        }
    }
}
