using FootballResults.Models.Predictions;
using FootballResults.Models.Users;
using FootballResults.WebApp.Database;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Collections.ObjectModel;

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
            await _dbContext.Messages.AddAsync(message);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Initialize(PredictionGame game)
        {
            _game = game;
            game.Messages.ToList().ForEach(m => Messages.Add(m));

            await base.JoinGroupAsync(game.GameID.ToString());
        }

        public override async Task SendMessageAsync(Message message)
        {
            await SaveMessage(message);
            await base.SendMessageToGroupAsync(_game!.GameID.ToString(), message);
        }
    }
}
