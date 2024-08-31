using Extensions;
using FootballResults.DatabaseUpdaters.Updaters;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FootballResults.DatabaseUpdaters.UpdaterMenu
{
    public static class UpdaterFactory
    {
        public static IEnumerable<Type> AvailableUpdaters
        {
            get => Assembly.GetExecutingAssembly()
                .GetUserDefinedClassesFromNamespace("FootballResults.DatabaseUpdaters.Updaters")
                .Where(type => type.GetCustomAttribute<UpdaterAttribute>() != null
                    && type.GetCustomAttribute<SupportedModesAttribute>() != null
                    && !type.GetCustomAttribute<SupportedModesAttribute>()!.SupportedModes.Contains(UpdaterMode.Helper));
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
