using Microsoft.AspNetCore.SignalR;

namespace FootballResults.WebApp.Hubs
{
    public class MessageHub<T> : Hub
    {
        public async Task SendMessageAsync(T message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task SendMessageToGroupAsync(string groupName, T message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }

        public async Task JoinGroupAsync(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
