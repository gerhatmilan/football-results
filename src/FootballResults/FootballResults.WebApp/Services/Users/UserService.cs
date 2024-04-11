using FootballResults.Models.Football;
using FootballResults.Models.General;
using FootballResults.Models.Users;
using FootballResults.WebApp.Components.Pages.MainMenu;
using FootballResults.WebApp.Database;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace FootballResults.WebApp.Services.Users
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _config;

        public UserService(HttpClient httpClient, AppDbContext dbContext, IConfiguration config)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
            _config = config;
        }

        public async Task<User?> GetUserAsync(int userID)
        {
            return await _dbContext.Users
               .Include(u => u.FavoriteLeagues)
               .Include(u => u.FavoriteTeams)
               .Include(u => u.Games)
               .FirstOrDefaultAsync(u => u.UserID == userID);
        }

        public async Task<bool> ModifyUserAsync(User user, SettingsModel settingsModel)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (user.Username != settingsModel.Username)
                    {
                        user.Username = settingsModel.Username;

                    }

                    if (user.ProfilePicturePath != settingsModel.ProfilePicturePath)
                    {
                        string saveFilePath = _config.GetValue<string>("Directories:ProfilePictures")!;
                        string saveFileName = $"{user.UserID}{Path.GetExtension(settingsModel.ProfilePicturePath)}";

                        // delete all old profile pictures for this user, regardless of extension
                        await FileManager.DeleteFilesWithNameAsync(saveFilePath, user.UserID.ToString());

                        // rename the uploaded profile picture file to the user's ID
                        await FileManager.MoveFileAsync(settingsModel.ProfilePicturePath, Path.Combine(saveFilePath, saveFileName));

                        user.ProfilePicturePath = saveFilePath + $"/{saveFileName}";
                    }

                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task AddToFavoriteLeaguesAsync(User user, League league)
        {
            await _dbContext.FavoriteLeagues.AddAsync(new FavoriteLeague { UserID = user.UserID, LeagueID = league.LeagueID });
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddToFavoriteTeamsAsync(User user, Team team)
        {
            await _dbContext.FavoriteTeams.AddAsync(new FavoriteTeam { UserID = user.UserID, TeamID = team.TeamID });
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveFromFavoriteLeaguesAsync(User user, League league)
        {
            var entity = await _dbContext.FavoriteLeagues.FindAsync(user.UserID, league.LeagueID);

            if (entity != null)
            {
                _dbContext.FavoriteLeagues.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveFromFavoriteTeamsAsync(User user, Team team)
        {
            var entity = await _dbContext.FavoriteTeams.FindAsync(user.UserID, team.TeamID);

            if (entity != null)
            {
                _dbContext.FavoriteTeams.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task GetGameDataForUserAsync(User user)
        {
            user = await _dbContext.Users
                .Include(u => u.Games)
                .Include(u => u.Messages)
                .Include(u => u.Predictions)
                .FirstAsync(u => u.UserID == user.UserID);
        }
    }
}
