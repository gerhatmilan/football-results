using FootballResults.DataAccess.Entities;
using FootballResults.Models.Application;
using FootballResults.Models.ViewModels.Application;

namespace FootballResults.WebApp.Services.Application
{
    public interface IApplicationService
    {
        public Task<ApiConfig> GetApiConfigAsync();
        public Task<ApplicationConfig> GetApplicationConfigAsync();
        public Task<IEnumerable<EndpointConfig>> GetEndpointConfigsAsync();
        public Task<Config> GetConfigAsync();
        public Task SaveConfig(Config config);
        public Task InitializeApplicationSettingsFormModelAsync(Config config, ApplicationSettingsFormModel applicationSettingsFormModel);
        public Task CopyApplicationSettingsFromModelAsync(Config config, ApplicationSettingsFormModel applicationSettingsFormModel);
    }
}
