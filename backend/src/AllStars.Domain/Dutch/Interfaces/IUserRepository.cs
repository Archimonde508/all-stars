using AllStars.Domain.User.Models;

namespace AllStars.Domain.Dutch.Interfaces;

public interface IUserRepository
{
    Task<AllStarUser?> GetOneAsync(string nickName, CancellationToken token);

    Task<List<AllStarUser>> GetManyAsync(IEnumerable<string> nickNames, CancellationToken token);

    Task<List<string>> GetAllNicknames(CancellationToken token);

    Task CreateDefaultUsersAsync(CancellationToken token);

    Task CreateUserAsync(AllStarUser user, CancellationToken token);
}
