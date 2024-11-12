using FootballResults.DataAccess.Entities;
using FootballResults.DataAccess.Entities.Football;

namespace FootballResults.Models.Application
{
    public class Config
    {
        public ApiConfig ApiConfig { get; set; }
        public ApplicationConfig ApplicationConfig { get; set; }
        public IEnumerable<EndpointConfig> EndpointConfigs { get; set; }
        public IEnumerable<League> Leagues { get; set; }
    }
}
