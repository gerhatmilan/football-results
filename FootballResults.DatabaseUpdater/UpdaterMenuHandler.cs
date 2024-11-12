using FootballResults.Models.Updaters;
using FootballResults.Models.Updaters.Services;

namespace FootballResults.DatabaseUpdater
{
    public class UpdaterMenuHandler
    {
        private const string GREEN = "\x1b[92m";
        private const string DEFAULT = "\u001b[0m";

        private const string DESCRIPTION = "This is a simple console interface to manually fetch data for the database.\n" +
            $"After running an updater, press any key and you can continue running the updaters.\n" +
            $"\n{GREEN}DownArrow{DEFAULT} / {GREEN}S{DEFAULT}: navigate down" +
            $"\n{GREEN}UpArrow{DEFAULT} / {GREEN}W{DEFAULT}: navigate up" +
            $"\n{GREEN}Enter{DEFAULT}: select option" +
            $"\n{GREEN}Q{DEFAULT}: quit";

        private UpdaterRunnerService _updaterRunnerService;

        public string Description => DESCRIPTION;
        public int SelectedOption { get; set; }
        public int CursorPositionLeft { get; set; }
        public int CursorPositionTop { get; set; }
        public ConsoleKeyInfo SelectedKey { get; set; }

        public UpdaterMenuHandler(UpdaterRunnerService updaterRunnerService)
        {
            _updaterRunnerService = updaterRunnerService;
            SelectedOption = 0;
            Console.CursorVisible = false;
        }

        public void SaveCursorPosition()
        {
            (CursorPositionLeft, CursorPositionTop) = Console.GetCursorPosition();
        }

        public void ResetCursorPosition()
        {
            Console.SetCursorPosition(CursorPositionLeft, CursorPositionTop);
        }

        public void ResetConsole(bool waitForInput = false)
        {
            if (waitForInput) Console.ReadKey();
            Console.Clear();
            ShowDescription();
            SaveCursorPosition();

            if (_updaterRunnerService.SelectedUpdater != null && _updaterRunnerService.MenuMode == UpdaterMenuMode.ShowUpdaters)
            {
                SelectedOption = _updaterRunnerService.SelectedUpdaterIndex;
            }
            else if (_updaterRunnerService.SelectedMode != null && _updaterRunnerService.MenuMode == UpdaterMenuMode.ShowModes)
            {
                SelectedOption = _updaterRunnerService.SelectedModeIndex;
            }
            else
            {
                SelectedOption = 0;
            }
        }

        public void ShowDescription()
        {
            Console.WriteLine(DESCRIPTION);
        }

        public void ShowOption(int index, string option)
        {
            Console.WriteLine($"{(SelectedOption == index ? $"{GREEN}>{DEFAULT} " : "  ")}{option}");
        }

        private void ShowUpdaterMenu()
        {
            Type[] availableUpdaters = IUpdaterRunnerService.AvailableUpdaters.ToArray();

            Console.WriteLine("\nAvailable updaters\n");

            for (int i = 0; i < IUpdaterRunnerService.AvailableUpdaters.Count(); i++)
            {
                ShowOption(i, availableUpdaters[i].Name);
            }

            Console.WriteLine();
        }

        private void ShowModesMenu(IUpdater selectedUpdater)
        {
            UpdaterMode[] supportedModes = selectedUpdater.SupportedModes.ToArray();

            Console.WriteLine($"\n{selectedUpdater.GetType().Name} > Available modes\n");

            for (int i = 0; i < selectedUpdater.SupportedModes.Count(); i++)
            {
                ShowOption(i, supportedModes[i].ToString());
            }

            Console.WriteLine();
        }

        public void ShowMenu(IUpdater? selectedUpdater, UpdaterMode? selectedMode)
        {
            switch (_updaterRunnerService.MenuMode)
            {
                case UpdaterMenuMode.ShowUpdaters:
                    ShowUpdaterMenu();
                    break;
                case UpdaterMenuMode.ShowModes:
                    ShowModesMenu(selectedUpdater!);
                    break;
            }
        }

        public void WaitForInput()
        {
            SelectedKey = Console.ReadKey(intercept: true);
        }

        public DateTime? GetDateFromUser()
        {
            Console.Write("Date: ");
            string? input = Console.ReadLine();
            DateTime date;

            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                return null;
            }
            else if (!DateTime.TryParse(input, out date))
            {
                Console.Write("Invalid input format!");
                Thread.Sleep(2000);
                return null;
            }

            return date;
        }

        public int? GetIDFromUser()
        {
            Console.Write("ID: ");
            string? input = Console.ReadLine();
            int id;

            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                return null;
            }
            else if (!int.TryParse(input, out id))
            {
                Console.Write("Invalid input format!");
                Thread.Sleep(2000);
                return null;
            }

            return id;
        }

        public int? GetSeasonFromUser()
        {
            Console.Write("Season: ");
            string? input = Console.ReadLine();
            int season;

            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                return null;
            }
            else if (!int.TryParse(input, out season))
            {
                Console.Write("Invalid input format!");
                Thread.Sleep(2000);
                return null;
            }

            return season;
        }


        public TimeSpan? GetLastUpdateBoundaryFromUser()
        {
            Console.WriteLine("The update will occur only when more time has passed since the last update than the given value.");
            Console.WriteLine("Format: [d.]hh:mm:ss");
            Console.Write("Maximum elapsed time since the last update: ");
            string? input = Console.ReadLine();
            TimeSpan maximumElapsedTime;

            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                return null;
            }
            else if (!TimeSpan.TryParse(input, out maximumElapsedTime))
            {
                Console.Write("Invalid input format!");
                Thread.Sleep(2000);
                return null;
            }

            return maximumElapsedTime;
        }
    }
}
