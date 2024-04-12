using FootballResults.Models.Predictions;
using FootballResults.Models.Users;
using FootballResults.WebApp.Services.Chat;
using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;

namespace FootballResults.WebApp.Components.Utilities
{
    public class ChatWindowBase : ComponentBase, IAsyncDisposable
    {
        [Inject]
        protected IChatService<Message> GameChatService { get; set; } = default!;

        [Inject]
        protected IUserService UserService { get; set; } = default!;

        [Inject]
        protected IJSRuntime JSRuntime { get; set; } = default!;

        [CascadingParameter(Name = "User")]
        protected User? User { get; set; }

        [CascadingParameter(Name = "Game")]
        protected PredictionGame? Game { get; set; }

        protected string? Message { get; set; }

        protected ElementReference ChatWindowReference;

        protected int LastScrollPosition = -1;
        protected bool ShouldScrollToBottom = true;

        protected override async Task OnInitializedAsync()
        {
            if (Game != null && User != null)
            {
                await GameChatService.StartAsync();
                GameChatService.NewMessageArrived += ChatService_NewMessageArrived;
                await ((GameChatService)GameChatService).Initialize(Game);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await JSRuntime.InvokeAsync<int>("scrollToBottom", ChatWindowReference, LastScrollPosition);
        }

        protected async void ChatService_NewMessageArrived(object? sender, EventArgs e)
        {
            await InvokeAsync(StateHasChanged);
            LastScrollPosition = await JSRuntime.InvokeAsync<int>("scrollToBottom", ChatWindowReference, LastScrollPosition);
        }

        protected void ResetMessage()
        {
            Message = null;
        }

        protected async Task SendMessageAsync(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" && !String.IsNullOrEmpty(Message) && User != null && Game != null)
            {
                var message = new Message
                {
                    UserID = User.UserID,
                    Text = Message,
                    MatchID = null,
                    GameID = Game.GameID
                };

                await ((GameChatService)GameChatService).SendMessageAsync(message);
                ResetMessage();
            }
        }

        public async ValueTask DisposeAsync()
        {
            GameChatService.NewMessageArrived -= ChatService_NewMessageArrived;
            await GameChatService.DisposeAsync();
        }
    }
}
