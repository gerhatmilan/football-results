using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.Models.Config;
using FootballResults.Models.Files;
using FootballResults.Models.ViewModels.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FootballResults.WebApp.Services.Users
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;
        private readonly IOptions<ApplicationConfig> _applicationSettings;
        private const string WWWROOT = "wwwroot";

        public UserService(AppDbContext dbContext, IOptions<ApplicationConfig> applicationSettings)
        {
            _dbContext = dbContext;
            _applicationSettings = applicationSettings;
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
               .Include(u => u.Participations)
                    .ThenInclude(p => p.Predictions)
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
                        string saveFilePath = _applicationSettings.Value.ProfilePicturesDirectory;
                        string saveFileName = $"{user.ID}{Path.GetExtension(settingsModel.ProfilePicturePath)}";

                        // delete all old profile pictures for this user, regardless of extension
                        await FileManager.DeleteFilesWithNameAsync(Path.Combine(WWWROOT, saveFilePath), user.ID.ToString());

                        // rename the uploaded profile picture file to the user's ID
                        await FileManager.MoveFileAsync(Path.Combine(WWWROOT, settingsModel.ProfilePicturePath), Path.Combine(WWWROOT, saveFilePath, saveFileName));

                        user.ProfilePicturePath = Path.Combine(saveFilePath, saveFileName);
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

        public async Task AddToFavoriteLeaguesAsync(int userID, int leagueID)
        {
            await _dbContext.FavoriteLeagues.AddAsync(new FavoriteLeague { UserID = userID, LeagueID = leagueID });
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddToFavoriteTeamsAsync(int userID, int teamID)
        {
            await _dbContext.FavoriteTeams.AddAsync(new FavoriteTeam {UserID = userID, TeamID = teamID });
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveFromFavoriteLeaguesAsync(int userID, int leagueID)
        {
            var entity = await _dbContext.FavoriteLeagues.FirstOrDefaultAsync(i => i.UserID == userID && i.LeagueID == leagueID);

            if (entity != null)
            {
                _dbContext.FavoriteLeagues.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveFromFavoriteTeamsAsync(int userID, int teamID)
        {
            var entity = await _dbContext.FavoriteTeams.FirstOrDefaultAsync(i => i.UserID == userID && i.TeamID == teamID);

            if (entity != null)
            {
                _dbContext.FavoriteTeams.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
