using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace FootballResults.WebApp.Services
{
    public abstract class HubService : IHubService
    {
        protected NavigationManager _navigationManager;
        protected HubConnection? _hubConnection;

        public virtual bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public HubService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public virtual async Task StartAsync()
        {
            if (!IsConnected && _hubConnection != null) await _hubConnection.StartAsync();
        }

        public virtual async Task StopAsync()
        {
            if (IsConnected && _hubConnection != null) await _hubConnection.StopAsync();
        }

        public virtual async ValueTask DisposeAsync()
        {
            await StopAsync();
        }
    }
}
