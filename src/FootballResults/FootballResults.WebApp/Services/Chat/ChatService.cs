using FootballResults.Models.Users;
using FootballResults.WebApp.Database;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FootballResults.WebApp.Services.Chat
{
    public class ChatService<T> : IChatService<T>
    {
        private HubConnection _hubConnection;
        private NavigationManager _navigationManager;

        public ICollection<T> Messages { get; set; }

        public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;

        public event EventHandler? NewMessageArrived;

        private void OnNewMessageArrived()
        {
            NewMessageArrived?.Invoke(this, new EventArgs());
        }

        public ChatService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/chathub"))
                .Build();

            Messages = new List<T>();

            _hubConnection.On<T>("ReceiveMessage", (message) =>
            {
                Messages.Add(message);
                OnNewMessageArrived();
            });
        }
        
        public async Task StartAsync()
        {
            if (!IsConnected)
                await _hubConnection.StartAsync();
        }

        public async Task StopAsync()
        {
            if (IsConnected)
                await _hubConnection.StopAsync();
        }

        public virtual async Task JoinGroupAsync(string groupName)
        {
            await _hubConnection.SendAsync("JoinGroupAsync", groupName);
        }

        public virtual async Task SendMessageAsync(T message)
        {
            await _hubConnection.SendAsync("SendMessageAsync", message);
        }

        public virtual async Task SendMessageToGroupAsync(string groupName, T message)
        {
            await _hubConnection.SendAsync("SendMessageToGroupAsync", groupName, message);
        }

        public async ValueTask DisposeAsync()
        {
            await StopAsync();
            Messages.Clear();
        }
    }
}
