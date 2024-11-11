using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities;
using FootballResults.DataAccess.Models;
using FootballResults.Models.Application;
using FootballResults.Models.ViewModels.Application;
using FootballResults.WebApp.Components.Forms;
using FootballResults.WebApp.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FootballResults.WebApp.Services.Application
{
    public class ApplicationService : IApplicationService
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public ApplicationService(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<ApiConfig> GetApiConfigAsync()
        {
            return await _dbContext.ApiConfig.OrderBy(i => i.ID).FirstAsync();
        }

        public async Task<ApplicationConfig> GetApplicationConfigAsync()
        {
            return await _dbContext.ApplicationConfig.OrderBy(i => i.ID).FirstAsync();
        }

        public async Task<IEnumerable<EndpointConfig>> GetEndpointConfigsAsync()
        {
            return await _dbContext.EndpointConfig.ToListAsync();
        }

        public async Task<Config> GetConfigAsync()
        {
            ApiConfig apiConfig = await GetApiConfigAsync();
            ApplicationConfig applicationConfig = await GetApplicationConfigAsync();
            IEnumerable<EndpointConfig> endpointConfigs = await GetEndpointConfigsAsync();

            return new Config
            {
                ApiConfig = apiConfig,
                ApplicationConfig = applicationConfig,
                EndpointConfigs = endpointConfigs
            };
        }

        public async Task InitializeApplicationSettingsFormModelAsync(Config config, ApplicationSettingsFormModel applicationSettingsFormModel)
        {
            applicationSettingsFormModel.ApiKeyBoundValue = await CryptoHelper.DecryptAsync(config.ApiConfig.ApiKey, _configuration.GetSection(Defaults.FootballApiKeyEncryptionKey).Value);
        }

        public async Task CopyApplicationSettingsFromModelAsync(Config config, ApplicationSettingsFormModel applicationSettingsFormModel)
        {
            config.ApiConfig.ApiKey = await CryptoHelper.EncryptAsync(applicationSettingsFormModel.ApiKeyBoundValue, _configuration.GetSection(Defaults.FootballApiKeyEncryptionKey).Value);
        }

        public async Task SaveConfig(Config config)
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
