using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.DataAccess;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Services.Chat
{
    public class GameChatService : ChatService<Message>
    {
        private AppDbContext _dbContext;
        private PredictionGame? _game;

        public GameChatService(AppDbContext dbContext, NavigationManager navigationManager) : base(navigationManager)
        {
            _dbContext = dbContext;
        }

        private async Task SaveMessage(Message message)
        {
            message.SentAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            await _dbContext.Messages.AddAsync(message);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Initialize(PredictionGame game)
        {
            _game = game;

            foreach (var message in game.Messages)
            {
                Messages.Add(message);
            }

            await base.JoinGroupAsync(game.ID.ToString());
        }

        public override async Task SendMessageAsync(Message message)
        {
            await SaveMessage(message);
            await base.SendMessageToGroupAsync(_game!.ID.ToString(), message);
        }
    }
}
