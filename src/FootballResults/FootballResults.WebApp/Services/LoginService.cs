using FootballResults.WebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FootballResults.WebApp.Services
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

        public async Task<LoginResult> AuthenticateUserAsync(User user)
        {
            User? userInDatabase = await GetUserAsync(user);

            if (userInDatabase == null)
                return LoginResult.UserNotFound;

            if (!IsCorrectPassword(user, userInDatabase))
                return LoginResult.InvalidPassword;

            return LoginResult.Success;
        }

        public async Task<User?> GetUserAsync(User user)
        {
            return await _dbContext.Users.Where(u => u.Username == user.Username).FirstOrDefaultAsync();
        }
    }
}
