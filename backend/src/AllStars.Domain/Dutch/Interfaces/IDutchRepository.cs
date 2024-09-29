using AllStars.Domain.Dutch.Models;

namespace AllStars.Domain.Dutch.Interfaces;

public interface IDutchRepository
{
    Task UpdateOne(DutchScore score, int points, CancellationToken token);

    //Task UpdateMany(List<DutchScore> scores, CancellationToken token);

    /// <summary>
    /// Adds a collection of DutchScores to the database.
    /// </summary>
    /// <param name="game">Game entity to add.</param>
    /// <param name="scores">The collection of DutchScore entities to add.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task.</returns>
    Task CreateMany(DutchGame game, IEnumerable<DutchScore> scores, CancellationToken token);

    /// <summary>
    /// Retrieves DutchScores by the player's nickname.
    /// </summary>
    /// <param name="nickName">The player's nickname.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task with a result of a collection of DutchScore entities.</returns>
    Task<IEnumerable<DutchScore>> GetUserScores(string nickName, CancellationToken token);

    Task<DutchScore?> GetOneScore(Guid gameId, string nickName, CancellationToken token);

    Task<List<DutchScore>> GetScoresByGame(Guid gameId, CancellationToken token);

    /// <summary>
    /// Retrieves all DutchScore entities.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task with a result of a collection of DutchScore entities.</returns>
    Task<IEnumerable<DutchScore>> GetAll(CancellationToken token);

    Task<int> UpdateManyScores(List<DutchScore> scores, CancellationToken token);
}