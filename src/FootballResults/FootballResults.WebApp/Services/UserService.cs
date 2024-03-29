using FootballResults.WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballResults.WebApp.Services
{
    public class UserService : IUserService
    {
        private AppDbContext _dbContext;
        public UserService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetUserIncludingFavoritesAsync(int userID)
        {
            return await _dbContext.Users
                .Include(u => u.FavoriteLeagues)
                .Include(u => u.FavoriteTeams)
                .FirstOrDefaultAsync(u => u.UserID == userID);
        }

        public async Task<User?> GetUserIncludingPredictionGamesAsync(int userID)
        {
            return await _dbContext.Users
                .Include(u => u.PredictionGames)
                .FirstOrDefaultAsync(u => u.UserID == userID);
        }

        public async Task AddToFavoriteLeaguesAsync(int userID, int leagueID)
        {
            await _dbContext.FavoriteLeagues.AddAsync(new FavoriteLeague { UserID = userID, LeagueID = leagueID });
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddToFavoriteTeamsAsync(int userID, int teamID)
        {
            await _dbContext.FavoriteTeams.AddAsync(new FavoriteTeam { UserID = userID, TeamID = teamID});
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveFromFavoriteLeaguesAsync(int userID, int leagueID)
        {
            var entity = await _dbContext.FavoriteLeagues.FindAsync(userID, leagueID);

            if (entity != null)
            {
                _dbContext.FavoriteLeagues.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveFromFavoriteTeamsAsync(int userID, int teamID)
        {
            var entity = await _dbContext.FavoriteTeams.FindAsync(userID, teamID);

            if (entity != null)
            {
                _dbContext.FavoriteTeams.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
