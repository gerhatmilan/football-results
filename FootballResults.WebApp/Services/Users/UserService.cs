using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.Models.Files;
using FootballResults.Models.ViewModels.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FootballResults.WebApp.Services.Users
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;
        private readonly ApplicationConfig _applicationConfig;
        private const string WWWROOT = "wwwroot";

        public UserService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _applicationConfig = dbContext.ApplicationConfig.OrderBy(i => i.ID).First();
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

        public async Task<bool> ModifyUserAsync(User user, SettingsModel settingsModel)
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
                        {
                            settingsModel.UsernameAlreadyInUseError = true;
                            return false;
                        }
                    }

                    if (user.ProfilePicturePath != settingsModel.ImagePath)
                    {
                        string saveFilePath = _applicationConfig.ProfilePicturesDirectory;
                        string saveFileName = $"{user.ID}{Path.GetExtension(settingsModel.ImagePath)}";

                        // delete all old profile pictures for this user, regardless of extension
                        await FileManager.DeleteFilesWithNameAsync(Path.Combine(WWWROOT, saveFilePath), user.ID.ToString());

                        // rename the uploaded profile picture file to the user's ID
                        await FileManager.MoveFileAsync(Path.Combine(WWWROOT, settingsModel.ImagePath), Path.Combine(WWWROOT, saveFilePath, saveFileName));

                        user.ProfilePicturePath = Path.Combine(saveFilePath, saveFileName);
                    }

                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();
                    
                    settingsModel.Success = true;
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    settingsModel.Error = true;
                    return false;
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
