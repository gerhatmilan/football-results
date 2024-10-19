using FootballResults.DataAccess.Entities.Users;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace FootballResults.WebApp.Services
{
    public interface IMessageService<T> : IHubService
    {
        public event EventHandler<T>? NewMessageArrived;

        public Task JoinGroupAsync(string groupName);
        public Task SendMessageAsync(T message);
        public Task SendMessageToGroupAsync(string groupName, T message);
    }
}
