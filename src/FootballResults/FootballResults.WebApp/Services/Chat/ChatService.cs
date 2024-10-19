using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.DataAccess;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Services.Chat
{
    public class ChatService : MessageService<Message>
    {
        private AppDbContext _dbContext;

        public ChatService(AppDbContext dbContext, NavigationManager navigationManager) : base(navigationManager, "/chathub")
        {
            _dbContext = dbContext;
        }

        private async Task<Message> SaveMessageAsync(Message message)
        {
            Message savedMessage = (await _dbContext.Messages.AddAsync(message)).Entity;
            await _dbContext.SaveChangesAsync();

            // load user property
            _dbContext.Entry(savedMessage).Reference(m => m.User).Load();
            return savedMessage;
        }

        public override async Task SendMessageToGroupAsync(string group, Message message)
        {
            Message savedMessage = await SaveMessageAsync(message);
            await base.SendMessageToGroupAsync(group, savedMessage);
        }

        public override async Task SendMessageAsync(Message message)
        {
            Message savedMessage = await SaveMessageAsync(message);
            await base.SendMessageAsync(savedMessage);
        }
    }
}
