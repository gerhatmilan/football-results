using Microsoft.JSInterop;

namespace FootballResults.WebApp.Services.Time
{
    public class ClientTimeService : IClientTimeService
    {
        private IJSRuntime _jsRuntime;
        private TimeSpan? _clientUtcDiff;

        public ClientTimeService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<TimeSpan> GetClientUtcDiffAsync()
        {
            if (_clientUtcDiff == null)
            {
                string clientDateString = await _jsRuntime.InvokeAsync<string>("getClientDate");
                DateTime utcTime = DateTime.UtcNow;
                var clientDate = DateTime.Parse(clientDateString);
                TimeSpan clientUtcDiff = clientDate - utcTime;

                // round up diff based on hour
                _clientUtcDiff =  TimeSpan.FromHours((int)(Math.Round((clientUtcDiff.Hours * 60 + clientUtcDiff.Minutes) / 60.0)));
            }

            return _clientUtcDiff.Value;
        }

        public async Task<DateTime> GetClientDateAsync()
        {
            string clientDateString = await _jsRuntime.InvokeAsync<string>("getClientDate");
            var clientDate = DateTime.Parse(clientDateString);

            return clientDate;
        }
    }
}
