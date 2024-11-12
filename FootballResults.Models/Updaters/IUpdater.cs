using System.Reflection;

namespace FootballResults.Models.Updaters
{
    public interface IUpdater
    {
        public IEnumerable<UpdaterMode> SupportedModes { get; }

        public Task StartAsync();
        public Task StartAsync(UpdaterMode mode, params object[] modeParameters);
        public static IEnumerable<UpdaterMode> GetSupportedModesForType(Type type)
        {
            SupportedModesAttribute attribute = type.GetCustomAttribute<SupportedModesAttribute>();

            if (attribute == null)
                return new List<UpdaterMode>();
            else
                return attribute.SupportedModes;
        }
    }
}
