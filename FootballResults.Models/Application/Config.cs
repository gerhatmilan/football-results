using FootballResults.DataAccess.Entities;

namespace FootballResults.Models.Application
{
    public class Config
    {
        public ApiConfig ApiConfig { get; set; }
        public ApplicationConfig ApplicationConfig { get; set; }
        public IEnumerable<EndpointConfig> EndpointConfigs { get; set; }
    }
}
