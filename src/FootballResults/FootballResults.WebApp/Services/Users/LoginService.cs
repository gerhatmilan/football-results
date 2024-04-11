using FootballResults.Models.Users;
using FootballResults.WebApp.Database;
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

        private bool IsCorrectPassword(User providedUser, User actualUser)
        {
            return new PasswordHasher<User>().VerifyHashedPassword(providedUser, actualUser.Password!, providedUser.Password!) == PasswordVerificationResult.Success;
        }

        public async Task<Tuple<User?, LoginResult>> AuthenticateUserAsync(User user)
        {
            User? userInDatabase = await GetUserAsync(user);

            if (userInDatabase == null)
                return Tuple.Create<User?, LoginResult>(null, LoginResult.UserNotFound);

            if (!IsCorrectPassword(user, userInDatabase))
                return Tuple.Create<User?, LoginResult>(null, LoginResult.InvalidPassword);

            return Tuple.Create<User?, LoginResult>(userInDatabase, LoginResult.Success);
        }

        public async Task<User?> GetUserAsync(User user)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Username == user.Username)
                .FirstOrDefaultAsync();
        }
    }
}
