using FootballResults.WebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FootballResults.WebApp.Services
{
    public class SignupService : ISignupService
    {  
        private AppDbContext _dbContext;

        public SignupService(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        private string GetHashedPassword(User user)
        {
            return new PasswordHasher<User>().HashPassword(user, user.Password!);
        }
        private async Task<bool> IsDupliateEmail(User user)
        {
            return await _dbContext!.Users.AnyAsync(u => u.Email == user.Email);
        }

        private async Task<bool> IsDuplicateUsername(User user)
        {
            return await _dbContext!.Users.AnyAsync(u => u.Username == user.Username);
        }

        public async Task<SignUpResult> RegisterUserAsync(User user)
        {
            if (await IsDupliateEmail(user))
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

            _dbContext.Add(hashedUser);
            await _dbContext.SaveChangesAsync();

            return SignUpResult.Success;
        }
    }
}
