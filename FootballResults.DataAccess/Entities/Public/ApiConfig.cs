using FootballResults.WebApp.Services.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballResults.DataAccess.Entities
{
    public class ApiConfig : EntityWithID
    {
        public string BaseAddress { get; set; }
        public string BaseAdressHeaderKey { get; set; }
        public byte[] ApiKey { get; set; }
        public string ApiKeyHeaderKey { get; set; }
        public int? RateLimit { get; set; }
        public bool BackupData { get; set; }

        public async Task<Dictionary<string, string>> GetRequestHeadersAsync(string apiEncryptionKey)
        {
            string apiKeyDecrypted = await CryptoHelper.DecryptAsync(ApiKey, apiEncryptionKey);

            return new Dictionary<string, string>
            {
                { BaseAdressHeaderKey, BaseAddress },
                { ApiKeyHeaderKey, apiKeyDecrypted }
            };
        }
    }
}
