using AllStars.Domain.Dutch.Interfaces;
using AllStars.Domain.Dutch.Models;
using AllStars.Domain.Dutch.Models.Commands;
using AllStars.Domain.Logs.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace AllStars.Application.Services.Dutch;

public class DutchService(IDutchRepository dutchRepository, IUserRepository userRepository, ILogRepository logRepository) : IDutchService
{
    public async Task<IEnumerable<DutchScore>> GetUserScores(string name, CancellationToken token) 
        => await dutchRepository.GetUserScores(name, token);

    public async Task<IEnumerable<DutchScore>> GetAll(CancellationToken token)
        => await dutchRepository.GetAll(token);

    public async Task<bool> UpdateOne(Guid gameId, string nickName, int points, CancellationToken token)
    {
        var scores = await dutchRepository.GetScoresByGame(gameId, token);
        if (scores.IsNullOrEmpty())
        {
            return false;
        }

        var updatedScore = scores.SingleOrDefault(s => s.Player.Nickname == nickName);
        if (updatedScore is null)
        {
            return false;
        }

        updatedScore.Points = points;
        scores = scores.OrderBy(s => s.Points).ToList();

        // Logic below assigns ties properly.
        int currentPosition = 1;
        for (int i = 0; i < scores.Count; i++)
        {
            if (i > 0 && scores[i].Points == scores[i - 1].Points)
            {
                scores[i].Position = scores[i - 1].Position;
            }
            else
            {
                scores[i].Position = currentPosition;
            }
            currentPosition++;
        }

        _ = await dutchRepository.UpdateManyScores(scores, token);
        return true;
    }

    public async Task CreateMany(CreateDutchGameCommand createDutchGameCommand, CancellationToken token)
    {
        var nickNames = createDutchGameCommand.ScorePairs
            .Select(x => x.NickName)
            .ToList();

        var users = await userRepository.GetManyAsync(nickNames, token);

        if (users.Count != nickNames.Count)
        {
            var foundUserNames = users.Select(u => u.Nickname);
            var missingNickNames = nickNames.Except(foundUserNames);

            throw new InvalidOperationException($"Cannot create game. Some users where not found: {string.Join(", ", missingNickNames)}");
        }
        
        var game = new DutchGame
        {
            Id = Guid.NewGuid(),
            Date = DateTime.Now,
            Comment = createDutchGameCommand.Comment
        };

        var usernames = users.ToDictionary(u => u.Nickname, u => u.Id);

        // Logic below assigns ties properly.
        var scoreGroups = createDutchGameCommand
            .ScorePairs
            .GroupBy(sp => sp.Score)
            .OrderBy(g => g.Key)
            .ToList();

        var scores = scoreGroups
            .SelectMany((group, index) =>
                group.Select(sp => new DutchScore
                {
                    Id = Guid.NewGuid(),
                    DutchGameId = game.Id,
                    PlayerId = usernames[sp.NickName],
                    Points = sp.Score,
                    Position = scoreGroups.Take(index).Sum(g => g.Count()) + 1
                }))
            .ToList();

        await dutchRepository.CreateMany(game, scores, token);

        await logRepository.AddDutchGameCreationLog(createDutchGameCommand, token);
    }
}
