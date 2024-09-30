using AllStars.Domain.User.Models;

namespace AllStars.Domain.User.Interfaces;

public interface IUserService
{
    Task RegisterUserAsync(AllStarUser user, string password, CancellationToken token);

    Task<IEnumerable<string>> GetAllNicknames(CancellationToken token);
}
