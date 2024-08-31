using FootballResults.DataAccess.Entities.Users;
using FootballResults.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FootballResults.WebApp.Services.Users
{
    public class SignupService : ISignupService
    {
        private AppDbContext _dbContext;

        public SignupService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        private string GetHashedPassword(User user)
        {
            return new PasswordHasher<User>().HashPassword(user, user.Password!);
        }
        private async Task<bool> IsDupliateEmailAsync(User user)
        {
            return await _dbContext!.Users.AnyAsync(u => u.Email == user.Email);
        }

        private async Task<bool> IsDuplicateUsername(User user)
        {
            return await _dbContext!.Users.AnyAsync(u => u.Username == user.Username);
        }

        public async Task<SignUpResult> RegisterUserAsync(User user)
        {
            if (await IsDupliateEmailAsync(user))
            {
                return SignUpResult.EmailAlreadyInUse;
            }
            if (await IsDuplicateUsername(user))
            {
                return SignUpResult.UsernameAlreadyInUse;
            }

            var hashedPassword = GetHashedPassword(user);
            var hashedUser = new User
            {
                Email = user.Email,
                Username = user.Username,
                Password = hashedPassword,
            };

            await _dbContext.AddAsync(hashedUser);
            await _dbContext.SaveChangesAsync();

            return SignUpResult.Success;
        }
    }
}
