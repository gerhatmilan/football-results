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

        public async Task<Tuple<User?, LoginResult>> AuthenticateUserAsync(string username, string password)
        {
            User? userInDatabase = await GetUserAsync(username);

            if (userInDatabase == null)
                return Tuple.Create<User?, LoginResult>(null, LoginResult.UserNotFound);

            if (!IsCorrectPassword(password, userInDatabase))
                return Tuple.Create<User?, LoginResult>(null, LoginResult.InvalidPassword);

            return Tuple.Create<User?, LoginResult>(userInDatabase, LoginResult.Success);
        }
    }
}
