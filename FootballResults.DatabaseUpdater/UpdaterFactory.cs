using Extensions;
using FootballResults.Models.Updaters;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FootballResults.DatabaseUpdater
{
    public static class UpdaterFactory
    {
        public static IEnumerable<Type> AvailableUpdaters
        {
            get => Assembly.Load("FootballResults.Models")
                .GetUserDefinedClassesFromNamespace("FootballResults.Models.Updaters")
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
