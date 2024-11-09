using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace FootballResults.Models.Api
{
    public class WebApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public string BaseAddress => _httpClient.BaseAddress.ToString();

        public WebApiClient(string baseAddress, ILogger logger = null)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
            _logger = logger;
        }


        public async Task<string> GetAsync(string endpoint, Dictionary<string, string> headers = null)
        {
            foreach (var header in headers)
            {
                if (!_httpClient.DefaultRequestHeaders.Contains(header.Key))
                {
                    _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<T> GetAsync<T>(string endpoint, Dictionary<string, string> headers = null)
        {
            var jsonResponse = await GetAsync(endpoint, headers);
            return JsonConvert.DeserializeObject<T>(jsonResponse);
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var jsonContent = JsonConvert.SerializeObject(data);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, contentString);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(jsonResponse);
        }
    }
}
