using FootballResults.WebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FootballResults.WebApp.Services
{
    public enum SignUpResult
    {
        Success,
        EmailAlreadyInUse,
        UsernameAlreadyInUse,
        Error
    }

    public class SignupService : ISignupService
    {
        
        private AppDbContext _dbContext;

        public SignupService(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public string GetHashedPassword(User user)
        {
            return new PasswordHasher<User>().HashPassword(user, user.Password!);
        }

        public async Task<SignUpResult> RegisterUserAsync(User user)
        {
            if (IsDupliateEmail(user))
            {
                return SignUpResult.EmailAlreadyInUse;
            }
            if (IsDuplicateUsername(user))
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

            try
            {
                _dbContext.Add(hashedUser);
                await _dbContext.SaveChangesAsync();

                return SignUpResult.Success;
            }
            catch (Exception)
            {
                return SignUpResult.Error;
            }
        }

        public bool IsDupliateEmail(User user)
        {
            return _dbContext!.Users.Any(u => u.Email == user.Email);
        }

        public bool IsDuplicateUsername(User user)
        {
            return _dbContext!.Users.Any(u => u.Username == user.Username);
        }
    }
}
