using FootballResults.DataAccess.Entities.Users;
using FootballResults.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FootballResults.WebApp.Services.Users
{
    public class LoginService : ILoginService
    {
        private AppDbContext _dbContext;

        public LoginService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private bool IsCorrectPassword(string providedPassword, User actualUser)
        {
            return new PasswordHasher<User>().VerifyHashedPassword(actualUser, actualUser.Password!, providedPassword) == PasswordVerificationResult.Success;
        }

        private async Task<User?> GetUserAsync(string username)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> AuthenticateUserAsync(string username, string password)
        {
            User? userInDatabase = await GetUserAsync(username);

            if (userInDatabase == null)
            {
                return null;
            }
            if (!IsCorrectPassword(password, userInDatabase))
            {
                return null;
            }

            return userInDatabase;
        }
    }
}
