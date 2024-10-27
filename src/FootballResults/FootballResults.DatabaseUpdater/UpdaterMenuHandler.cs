using FootballResults.Models.Config;
using FootballResults.Models.Updaters;

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

        private UpdaterMenuMode _menuMode;

        private IEnumerable<IncludedLeagueRecord> IncludedLeagues { get; set; }

        public string Description => DESCRIPTION;
        public int SelectedOption { get; set; }
        public int CursorPositionLeft { get; set; }
        public int CursorPositionTop { get; set; }
        public ConsoleKeyInfo SelectedKey { get; set; }
        public UpdaterMenuMode MenuMode
        {
            get => _menuMode;
            set
            {
                _menuMode = value;
                SelectedOption = 0;
            }
        }

        public UpdaterMenuHandler(IEnumerable<IncludedLeagueRecord> includedLeagues)
        {
            IncludedLeagues = includedLeagues;
            SelectedOption = 0;
            Console.CursorVisible = false;
            _menuMode = UpdaterMenuMode.ShowUpdaters;
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
            Type[] availableUpdaters = UpdaterFactory.AvailableUpdaters.ToArray();

            Console.WriteLine("\nAvailable updaters\n");

            for (int i = 0; i < UpdaterFactory.AvailableUpdaters.Count(); i++)
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

        private void ShowLeaguesMenu(IUpdater selectedUpdater, UpdaterMode selectedMode)
        {
            Console.WriteLine($"\n{selectedUpdater.GetType().Name} > Available modes > {selectedMode} > Available leagues\n");

            for (int i = 0; i < IncludedLeagues.Count(); i++)
            {
                ShowOption(i, IncludedLeagues.ElementAt(i).Name);
            }

            Console.WriteLine();
        }

        public void ShowMenu(IUpdater? selectedUpdater, UpdaterMode? selectedMode)
        {
            switch (_menuMode)
            {
                case UpdaterMenuMode.ShowUpdaters:
                    ShowUpdaterMenu();
                    break;
                case UpdaterMenuMode.ShowModes:
                    ShowModesMenu(selectedUpdater!);
                    break;
                case UpdaterMenuMode.ShowLeagues:
                    ShowLeaguesMenu(selectedUpdater!, selectedMode!.Value);
                    break;
            }
        }

        public void WaitForInput()
        {
            SelectedKey = Console.ReadKey(intercept: true);
        }

        public DateTime GetDateFromUser()
        {
            Console.Write("Date: ");
            string? input = Console.ReadLine();

            if (input != null && DateTime.TryParse(input, out DateTime date))
            {
                return date;
            }
            else
            {
                throw new InvalidDataException("Invalid date format.");
            }
        }

        public int GetTeamFromUser()
        {
            Console.Write("Team ID: ");
            string? input = Console.ReadLine();

            if (input != null && int.TryParse(input, out int id))
            {
                return id;
            }
            else
            {
                throw new InvalidDataException("Invalid input format.");
            }
        }

        public TimeSpan GetLastUpdateBoundaryFromUser()
        {
            Console.WriteLine("The update will occur only when more time has passed since the last update than the given value.");
            Console.WriteLine("Format: [d.]hh:mm:ss");
            Console.Write("Maximum elapsed time since the last update: ");
            string? input = Console.ReadLine();

            if (input != null && TimeSpan.TryParse(input, out TimeSpan maximumElapsedTime))
            {
                return maximumElapsedTime;
            }
            else
            {
                throw new InvalidDataException("Invalid format.");
            }
        }
    }
}
