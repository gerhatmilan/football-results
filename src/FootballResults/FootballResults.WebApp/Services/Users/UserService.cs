using FootballResults.Models.Football;
using FootballResults.Models.General;
using FootballResults.Models.Users;
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
                    user.Username = settingsModel.Username;

                    if (settingsModel.ProfilePicture != null)
                    {
                        var profilePicPath = _config.GetValue<string>("Directories:ProfilePictures")!;
                        user.ProfilePicturePath = Path.Combine(profilePicPath, $"{user.UserID}.png");
                        await ImageManager.SaveImageAsync(settingsModel.ProfilePicture, profilePicPath, $"{user.UserID}.png");
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

        public async Task AddToFavoriteLeaguesAsync(int userID, int leagueID)
        {
            await _dbContext.FavoriteLeagues.AddAsync(new FavoriteLeague { UserID = userID, LeagueID = leagueID });
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddToFavoriteTeamsAsync(int userID, int teamID)
        {
            await _dbContext.FavoriteTeams.AddAsync(new FavoriteTeam { UserID = userID, TeamID = teamID });
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
        public async Task<IEnumerable<League>> GetFavoriteLeaguesAsync(User? user)
        {
            var leagues = await _httpClient.GetFromJsonAsync<IEnumerable<League>>("api/leagues");

            if (leagues != null && user?.FavoriteLeagues != null)
            {
                var userFavorites = user.FavoriteLeagues.Select(fl => fl.LeagueID);
                return leagues.Where(l => userFavorites.Contains(l.LeagueID));
            }
            else
            {
                return Enumerable.Empty<League>();
            }
        }

        public async Task<IEnumerable<Team>> GetFavoriteTeamsAsync(User? user)
        {
            var teams = await _httpClient.GetFromJsonAsync<IEnumerable<Team>>("api/teams");

            if (teams != null && user?.FavoriteTeams != null)
            {
                var userFavorites = user.FavoriteTeams.Select(ft => ft.TeamID);
                return teams.Where(t => userFavorites.Contains(t.TeamID));
            }
            else
            {
                return Enumerable.Empty<Team>();
            }
        }
    }
}
