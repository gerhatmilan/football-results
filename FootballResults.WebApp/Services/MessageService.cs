using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace FootballResults.WebApp.Services
{
    public class MessageService<T> : HubService, IMessageService<T>
    {
        public event EventHandler<T>? NewMessageArrived;

        public MessageService(NavigationManager navigationManager, string hubEndpoint) : base(navigationManager)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri(hubEndpoint))
                .Build();

            _hubConnection.On<T>("ReceiveMessage", OnNewMessageArrived);
        }

        private void OnNewMessageArrived(T message)
        {
            NewMessageArrived?.Invoke(this, message);
        }

        public virtual async Task JoinGroupAsync(string groupName)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.SendAsync("JoinGroupAsync", groupName);
            }
            else
            {
                throw new InvalidOperationException("Hub connection is not initialized");
            }
        }

        public virtual async Task SendMessageAsync(T message)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.SendAsync("SendMessageAsync", message);
            }
            else
            {
                throw new InvalidOperationException("Hub connection is not initialized");
            }
        }

        public virtual async Task SendMessageToGroupAsync(string groupName, T message)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.SendAsync("SendMessageToGroupAsync", groupName, message);
            }
            else
            {
                throw new InvalidOperationException("Hub connection is not initialized");
            }
        }
    }
}
