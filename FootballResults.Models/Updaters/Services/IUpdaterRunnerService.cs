using Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FootballResults.Models.Updaters.Services
{
    public interface IUpdaterRunnerService
    {
        public IUpdater SelectedUpdater { get; }
        public UpdaterMode? SelectedMode { get; set; }
        public UpdaterMenuMode MenuMode { get; set; }

        public int SelectedUpdaterIndex { get; }
        public int SelectedModeIndex { get; }

        public IEnumerable<UpdaterMode> UpdaterModes => SelectedUpdater?.SupportedModes ?? Enumerable.Empty<UpdaterMode>();

        public Task RunUpdaterAsync(params object[] modeParameters);

        public void SetUpdater(string updaterName);

        public void SetUpdater(int index);

        public void SetUpdaterMode(int index);

        public void Reset();

        public static IEnumerable<Type> AvailableUpdaters
        {
            get => Assembly.Load("FootballResults.Models")
                .GetUserDefinedClassesFromNamespace("FootballResults.Models.Updaters")
                .Where(type => type.GetCustomAttribute<UpdaterAttribute>() != null
                    && type.GetCustomAttribute<SupportedModesAttribute>() != null
                    && !type.GetCustomAttribute<SupportedModesAttribute>()!.SupportedModes.Contains(UpdaterMode.Helper));
        }

        public static IUpdater CreateUpdater(Type updaterType, IServiceProvider services)
        {
            if (updaterType != null)
            {
                return (IUpdater)ActivatorUtilities.CreateInstance(services, updaterType);
            }
            else
            {
                throw new ArgumentException($"Updater does not exist with the provided name: {updaterType.Name}");
            }
        }

        public static IUpdater CreateUpdater(int updaterIndex, IServiceProvider services)
        {
            Type updaterType = AvailableUpdaters.ElementAt(updaterIndex);

            if (updaterType != null)
            {
                return (IUpdater)ActivatorUtilities.CreateInstance(services, updaterType);
            }
            else
            {
                throw new ArgumentException($"Updater does not exist with the provided index: {updaterIndex}");
            }
        }
    }
}
