using FootballResults.WebApp.Services.LiveUpdates;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Services.LiveUpdates
{
    public class UpdateNotificationService : MessageService<UpdateMessageType>
    {
        public UpdateNotificationService(NavigationManager navigationManager) : base(navigationManager, "/updatehub")
        {
        }
    }
}
