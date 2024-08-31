namespace FootballResults.DatabaseUpdaters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SupportedModesAttribute : Attribute
    {
        public UpdaterMode[] SupportedModes { get; }

        public SupportedModesAttribute(params UpdaterMode[] supportedModes)
        {
            SupportedModes = supportedModes;
        }
    }
}
