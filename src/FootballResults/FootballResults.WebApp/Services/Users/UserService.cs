using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.Models.General;
using FootballResults.Models.Users;
using Microsoft.EntityFrameworkCore;

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

        private async Task<bool> IsDuplicateUsername(string userName)
        {
            return await _dbContext!.Users.AnyAsync(u => u.Username == userName);
        }

        public async Task<User?> GetUserAsync(int userID)
        {
            return await _dbContext.Users
               .Include(u => u.FavoriteLeagues)
               .Include(u => u.FavoriteTeams)
               .Include(u => u.PredictionGames)
               .AsSplitQuery()
               .FirstOrDefaultAsync(u => u.ID == userID);
        }

        public async Task<ModifyUserResult> ModifyUserAsync(User user, SettingsModel settingsModel)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (user.Username != settingsModel.Username)
                    {
                        if (!await IsDuplicateUsername(settingsModel.Username))
                            user.Username = settingsModel.Username;
                        else
                            return ModifyUserResult.UsernameAlreadyInUse;
                    }

                    if (user.ProfilePicturePath != settingsModel.ProfilePicturePath)
                    {
                        string saveFilePath = _config.GetValue<string>("Directories:ProfilePictures")!;
                        string saveFileName = $"{user.ID}{Path.GetExtension(settingsModel.ProfilePicturePath)}";

                        // delete all old profile pictures for this user, regardless of extension
                        await FileManager.DeleteFilesWithNameAsync(saveFilePath, user.ID.ToString());

                        // rename the uploaded profile picture file to the user's ID
                        await FileManager.MoveFileAsync(settingsModel.ProfilePicturePath, Path.Combine(saveFilePath, saveFileName));

                        user.ProfilePicturePath = saveFilePath + $"/{saveFileName}";
                    }

                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();
                    return ModifyUserResult.Success;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return ModifyUserResult.Error;
                }
            }
        }

        public async Task AddToFavoriteLeaguesAsync(User user, League league)
        {
            await _dbContext.FavoriteLeagues.AddAsync(new FavoriteLeague { UserID = user.ID, LeagueID = league.ID });
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddToFavoriteTeamsAsync(User user, Team team)
        {
            await _dbContext.FavoriteTeams.AddAsync(new FavoriteTeam { UserID = user.ID, TeamID = team.ID });
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveFromFavoriteLeaguesAsync(User user, League league)
        {
            var entity = await _dbContext.FavoriteLeagues.FindAsync(user.ID, league.ID);

            if (entity != null)
            {
                _dbContext.FavoriteLeagues.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveFromFavoriteTeamsAsync(User user, Team team)
        {
            var entity = await _dbContext.FavoriteTeams.FindAsync(user.ID, team.ID);

            if (entity != null)
            {
                _dbContext.FavoriteTeams.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task GetGameDataForUserAsync(User user)
        {
            user = await _dbContext.Users
                .Include(u => u.PredictionGames)
                .Include(u => u.Messages)
                .Include(u => u.Predictions)
                .FirstAsync(u => u.ID == user.ID);
        }
    }
}
