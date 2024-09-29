using AllStars.Domain.Dutch.Interfaces;
using AllStars.Domain.Dutch.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AllStars.Infrastructure.Dutch.Repository;

public class DutchRepository(AppDbContext context) : IDutchRepository
{
    public async Task CreateMany(DutchGame game, IEnumerable<DutchScore> scores, CancellationToken token)
    {
        if (scores.Any(s => s.DutchGameId != game.Id))
        {
            throw new DataException("Could not insert Dutch Games. Id is not consistient.");
        }

        await context.DutchGames.AddAsync(game, token);
        await context.DutchScores.AddRangeAsync(scores, token);        
        await context.SaveChangesAsync(token);
    }

    public async Task UpdateOne(DutchScore score, int points, CancellationToken token)
    {
        score.Points = points;

        await context.SaveChangesAsync(token);
    }


    public async Task<IEnumerable<DutchScore>> GetAll(CancellationToken token)
    {
        return await context.DutchScores
            .Include(ds => ds.Player)
            .Include(ds => ds.Player)
            .ToListAsync(token);
    }

    public async Task<IEnumerable<DutchScore>> GetUserScores(string nickName, CancellationToken token)
    {
        return await context.DutchScores
            .Where(ds => ds.Player.Nickname == nickName)
            .Include(ds => ds.Player)
            .Include(ds => ds.Game)
            .ToListAsync(token);
    }

    public async Task<DutchScore?> GetOneScore(Guid gameId, string nickName, CancellationToken token)
    {
        return await context.DutchScores
          .Include(ds => ds.Game) // check if neccessery
          .Include(ds => ds.Player) // check if neccessery
          .Where(ds => ds.Game.Id == gameId)
          .Where(ds => ds.Player.Nickname == nickName)
          .SingleOrDefaultAsync(token);
    }

    public async Task<List<DutchScore>> GetScoresByGame(Guid gameId, CancellationToken token)
    {
        return await context.DutchScores
          .Include(ds => ds.Game) // check if neccessery
          .Include(ds => ds.Player) // check if neccessery
          .Where(ds => ds.Game.Id == gameId)
          .ToListAsync(token);
    }

    public async Task<int> UpdateManyScores(List<DutchScore> scores, CancellationToken token)
    {
        foreach (var score in scores)
        {
            context.Attach(score);

            context.Entry(score).State = EntityState.Modified;
        }

        return await context.SaveChangesAsync(token);
    }
}
