using FootballResults.Models.Predictions;
using FootballResults.Models.Users;
using FootballResults.WebApp.Services.Chat;
using FootballResults.WebApp.Services.Time;
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
        protected IClientTimeService ClientTimeService { get; set; } = default!;

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

        protected TimeSpan ClientUtcDiff { get; set; }
        protected DateTime ClientDate { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (Game != null && User != null)
            {
                await GameChatService.StartAsync();
                GameChatService.NewMessageArrived += ChatService_NewMessageArrived;
                await ((GameChatService)GameChatService).Initialize(Game);
            }

            ClientDate = await ClientTimeService.GetClientDateAsync();
            ClientUtcDiff = await ClientTimeService.GetClientUtcDiffAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender && LastScrollPosition == -1)
                await JSRuntime.InvokeVoidAsync("scrollToBottom", ChatWindowReference);
        }

        protected async void ChatService_NewMessageArrived(object? sender, EventArgs e)
        {
            await InvokeAsync(StateHasChanged);
            LastScrollPosition = await JSRuntime.InvokeAsync<int>("scrollToBottomIfLastScrollDown", ChatWindowReference, LastScrollPosition);
        }

        protected void ResetMessage()
        {
            Message = null;
        }

        protected bool MessageSentToday(Message message)
        {
            return message.SentAt.Add(ClientUtcDiff).Date == ClientDate.Date;
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
