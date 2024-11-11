using FootballResults.WebApp.Services;
using FootballResults.WebApp.Services.Chat;
using FootballResults.WebApp.Services.LiveUpdates;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Pages
{
    public abstract class LiveUpdatePageBase : ComponentBase, IAsyncDisposable
    {
        [Inject]
        protected IMessageService<UpdateMessageType> UpdateNotificationService { get; set; } = default!;

        /// <summary>
        /// Needed to prevent concurrent queries on the same db context at the same time
        /// </summary>
        protected SemaphoreSlim UpdateLock { get; } = new SemaphoreSlim(1, 1);

        protected override async Task OnInitializedAsync()
        {
            await InitializeUpdateNotificationServiceAsync();
        }

        protected async Task InitializeUpdateNotificationServiceAsync()
        {
            await UpdateNotificationService.StartAsync();
            UpdateNotificationService.NewMessageArrived += OnUpdateMessageReceivedAsync;
        }

        protected abstract void OnUpdateMessageReceivedAsync(object? sender, UpdateMessageType notificationType);

        public async ValueTask DisposeAsync()
        {
            UpdateNotificationService.NewMessageArrived -= OnUpdateMessageReceivedAsync;
            await UpdateNotificationService.DisposeAsync();
        }
    }
}
