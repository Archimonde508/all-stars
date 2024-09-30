using AllStars.Domain.Dutch.Interfaces;
using Microsoft.EntityFrameworkCore;
using AllStars.Domain.User.Models;

namespace AllStars.Infrastructure.User.Repository;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<AllStarUser?> GetOneAsync(string nickName, CancellationToken token)
    {
        return await context.Users
            .Where(u => u.Nickname == nickName)
            .FirstOrDefaultAsync(token);
    }

    public async Task CreateUserAsync(AllStarUser user, CancellationToken token)
    {
        await context.Users.AddAsync(user, token);
        await context.SaveChangesAsync(token);
    }

    public async Task CreateDefaultUsersAsync(CancellationToken token)
    {
        var user1 = new AllStarUser
        {
            Id = Guid.NewGuid(),
            BirthDate = DateTime.Now,
            Families = Families.Kolobrzeg | Families.Gdansk,
            Nickname = "Patols",
            FirstName = "Patryk",
            LastName = "Olszewski"
        };

        var user2 = new AllStarUser
        {
            Id = Guid.NewGuid(),
            BirthDate = DateTime.Now,
            Families = Families.Reda | Families.Gdansk,
            Nickname = "Kisiel Reda",
            FirstName = "Jakub",
            LastName = "Kisiel"
        };

        var user3 = new AllStarUser
        {
            Id = Guid.NewGuid(),
            BirthDate = DateTime.Now,
            Families = Families.Matematyka | Families.Gdansk,
            Nickname = "Hitlerjuden",
            FirstName = "Pawel",
            LastName = "Stankiewicz"
        };

        await context.Users.AddRangeAsync(user1, user2, user3);
        await context.SaveChangesAsync(token);
    }

    public async Task<List<AllStarUser>> GetManyAsync(IEnumerable<string> nickNames, CancellationToken token)
    {
        return await context.Users
            .Where(u => nickNames.Contains(u.Nickname))
            .ToListAsync(token);
    }

    public async Task<List<string>> GetAllNicknames(CancellationToken token)
    {
        return await context.Users
           .Select(u => u.Nickname)
           .ToListAsync(token);
    }
}
