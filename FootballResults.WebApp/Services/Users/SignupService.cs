using FootballResults.DataAccess.Entities.Users;
using FootballResults.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FootballResults.Models.ViewModels.Users;

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

        public async Task RegisterUserAsync(SignupModel model)
        {
            User user = new User
            {
                Email = model.Email,
                Username = model.Username,
                Password = model.Password
            };

            if (await IsDupliateEmailAsync(user))
            {
                model.EmailAlreadyInUseError = true;
                return;
            }
            if (await IsDuplicateUsername(user))
            {
                model.UsernameAlreadyInUseError = true;
                return;
            }

            var hashedPassword = GetHashedPassword(user);
            var hashedUser = new User
            {
                Email = user.Email,
                Username = user.Username,
                Password = hashedPassword,
                RegistrataionDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };

            try
            {
                _dbContext.Add(hashedUser);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                model.Error = true;
                return;
            }

            model.Success = true;
        }
    }
}
