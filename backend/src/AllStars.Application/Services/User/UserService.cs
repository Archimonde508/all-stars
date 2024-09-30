using AllStars.Domain.Dutch.Interfaces;
using AllStars.Domain.User.Interfaces;
using AllStars.Domain.User.Models;

namespace AllStars.Application.Services.User;

public class UserService(IUserRepository userRepository): IUserService
{
    public async Task<IEnumerable<string>> GetAllNicknames(CancellationToken token)
    {
        var users = await userRepository.GetAllNicknames(token);
        return users;
    }

    public async Task RegisterUserAsync(AllStarUser user, string password, CancellationToken token)
    {
        var existingUser = await userRepository.GetOneAsync(user.Nickname, token);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User already exists.");
        }

        var salt = AuthHelpers.GenerateSalt();
        var hashedPassword = AuthHelpers.HashPassword(password, salt);

        user.PasswordSalt = salt;
        user.PasswordHash = hashedPassword;

        await userRepository.CreateUserAsync(user, token);
    }
}
