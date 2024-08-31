using FootballResults.DataAccess.Entities.Users;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace FootballResults.WebApp.Services.Chat
{
    public interface IChatService<T> : IAsyncDisposable
    {
        ICollection<T> Messages { get; set; }
        bool IsConnected { get; }

        event EventHandler? NewMessageArrived;

        Task StartAsync();
        Task StopAsync();
        Task JoinGroupAsync(string groupName);
        Task SendMessageAsync(T message);
        Task SendMessageToGroupAsync(string groupName, T message);
    }
}
