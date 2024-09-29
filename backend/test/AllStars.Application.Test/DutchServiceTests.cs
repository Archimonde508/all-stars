using AllStars.Application.Services.Dutch;
using AllStars.Domain.Dutch.Interfaces;
using AllStars.Domain.Dutch.Models;
using AllStars.Domain.Dutch.Models.Commands;
using AllStars.Domain.Logs.Interfaces;
using AllStars.Domain.User.Models;
using FluentAssertions;
using Moq;
using static System.Formats.Asn1.AsnWriter;

namespace AllStars.Application.Test;

public class DutchServiceTests
{
    private readonly Mock<IDutchRepository> _dutchRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogRepository> _logRepositoryMock;
    private readonly DutchService _dutchService;
    private readonly CancellationToken _token = new CancellationToken();

    public DutchServiceTests()
    {
        _dutchRepositoryMock = new Mock<IDutchRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _logRepositoryMock = new Mock<ILogRepository>();
        _dutchService = new DutchService(_dutchRepositoryMock.Object, _userRepositoryMock.Object, _logRepositoryMock.Object);
    }

    [Fact]
    public async Task GetUserScores_ShouldReturnScores()
    {
        // Arrange
        var userName = "Pawlak";
        var dutchScores = new List<DutchScore>
        {
            new() { Id = Guid.NewGuid(), Points = 100 }
        };

        _dutchRepositoryMock
            .Setup(repo => repo.GetUserScores(userName, _token))
            .ReturnsAsync(dutchScores);

        // Act
        var result = await _dutchService.GetUserScores(userName, _token);

        // Assert
        result.Should().NotBeNull();
        result.First().Should().Be(dutchScores.First());
        _dutchRepositoryMock.Verify(repo => repo.GetUserScores(userName, _token), Times.Once);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllScores()
    {
        // Arrange
        var allScores = new List<DutchScore>
        {
            new() { Id = Guid.NewGuid(), Points = 200 },
            new() { Id = Guid.NewGuid(), Points = 300 }
        };

        _dutchRepositoryMock
            .Setup(repo => repo.GetAll(_token))
            .ReturnsAsync(allScores);

        // Act
        var result = await _dutchService.GetAll(_token);

        // Assert
        result.Should().NotBeNull();
        result.First().Should().Be(allScores.First());
        result.Skip(1).First().Should().Be(allScores.Skip(1).First());
        _dutchRepositoryMock.Verify(repo => repo.GetAll(_token), Times.Once);
    }

    [Theory]
    [InlineData(50, 100, 100, 1, 2, 2)]
    [InlineData(100, 100, 100, 1, 1, 1)]
    [InlineData(60, 40, 20, 3, 2, 1)]
    [InlineData(150, 30, 30, 3, 1, 1)]
    public async Task UpdateOne_ShouldAssignTies_WhenPlayersHaveTie(int score1, int score2, int newScore, int rank1, int rank2, int rank3)
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var updatedPlayerName = "Karolek";

        var existingScores = new List<DutchScore>()
        {
            new() { Id = Guid.NewGuid(), Player = new() {Nickname = "Matejus" }, Points = score1, Position = 0 },
            new() { Id = Guid.NewGuid(), Player = new() {Nickname = "Baron" }, Points = score2, Position = 0 },
            new() { Id = Guid.NewGuid(), Player = new() {Nickname = updatedPlayerName }, Points = 0, Position = 0 },
        };

        var updatedScores = new List<DutchScore>()
        {
            new() { Id = existingScores[0].Id, Player = existingScores[0].Player, Points = score1, Position = rank1 },
            new() { Id = existingScores[1].Id, Player = existingScores[1].Player, Points = score2, Position = rank2 },
            new() { Id = existingScores[2].Id, Player = existingScores[2].Player, Points = newScore, Position = rank3 },
        };

        _dutchRepositoryMock
            .Setup(repo => repo.GetScoresByGame(gameId, _token))
            .ReturnsAsync(existingScores);

        // Act
        var result = await _dutchService.UpdateOne(gameId, updatedPlayerName, newScore, _token);

        // Assert
        Assert.True(result);
        _dutchRepositoryMock.Verify(repo => repo.GetScoresByGame(gameId, _token), Times.Once);
        _dutchRepositoryMock.Verify(repo => repo.UpdateManyScores(
             It.Is<List<DutchScore>>(list =>
                 list.Count == updatedScores.Count &&
                 list.All(es => updatedScores.Any(ls =>
                     ls.Id == es.Id &&
                     ls.PlayerId == es.PlayerId &&
                     ls.Points == es.Points &&
                     ls.Position == es.Position))
             ), _token), Times.Once);
    }

    [Fact]
    public async Task UpdateOne_ShouldReturnFalse_WhenScoreListIsEmpty()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var updatedPlayerName = "Karolek";
        var points = 150;

        _dutchRepositoryMock
            .Setup(repo => repo.GetScoresByGame(gameId, _token))
            .ReturnsAsync(new List<DutchScore>());

        // Act
        var result = await _dutchService.UpdateOne(gameId, updatedPlayerName, points, _token);

        // Assert
        Assert.False(result);
        _dutchRepositoryMock.Verify(repo => repo.GetScoresByGame(gameId, _token), Times.Once);
        _dutchRepositoryMock.Verify(repo => repo.UpdateManyScores(It.IsAny<List<DutchScore>>(), _token), Times.Never);
    }

    [Fact]
    public async Task UpdateOne_ShouldReturnFalse_WhenScoreListDoesNotContainGivenUser()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var updatedPlayerName = "Karolek";
        var points = 150;

        _dutchRepositoryMock
            .Setup(repo => repo.GetScoresByGame(gameId, _token))
            .ReturnsAsync(new List<DutchScore>() { new() { Id = Guid.NewGuid(), Player = new () } });

        // Act
        var result = await _dutchService.UpdateOne(gameId, updatedPlayerName, points, _token);

        // Assert
        Assert.False(result);
        _dutchRepositoryMock.Verify(repo => repo.GetScoresByGame(gameId, _token), Times.Once);
        _dutchRepositoryMock.Verify(repo => repo.UpdateManyScores(It.IsAny<List<DutchScore>>(), _token), Times.Never);
    }

    [Theory]
    [InlineData(50, 100, 100, 1, 2, 2)]
    [InlineData(100, 100, 100, 1, 1, 1)]
    [InlineData(60, 40, 20, 3, 2, 1)]
    [InlineData(150, 30, 30, 3, 1, 1)]
    public async Task CreateMany_ShouldCreateGame_WhenAllUsersExist(int score1, int score2, int score3, int rank1, int rank2, int rank3)
    {
        // Arrange
        var comment = "New game";
        var command = new CreateDutchGameCommand
        {
            Comment = comment,
            ScorePairs =
            [
                new ScorePair { NickName = "User1", Score = score1 },
                new ScorePair { NickName = "User2", Score = score2 },
                new ScorePair { NickName = "User3", Score = score3 }
            ]
        };
        
        var users = new List<AllStarUser>
        {
            new() { Id = Guid.NewGuid(), Nickname = "User1" },
            new() { Id = Guid.NewGuid(), Nickname = "User2" },
            new() { Id = Guid.NewGuid(), Nickname = "User3" }
        };

        var dutchGameToBeCreated = new DutchGame()
        {
            Comment = comment,
            DutchScores =
            [
                new DutchScore() { Player = new() {Nickname = "User1"}, Points = score1, Position = rank1 },
                new DutchScore() { Player = new() {Nickname = "User2"}, Points = score2, Position = rank2 },
                new DutchScore() { Player = new() {Nickname = "User3"}, Points = score3, Position = rank3 },
            ]
        };

        _userRepositoryMock
            .Setup(repo => repo.GetManyAsync(It.IsAny<IEnumerable<string>>(), _token))
            .ReturnsAsync(users);

        // Act
        await _dutchService.CreateMany(command, _token);

        // Assert
        _dutchRepositoryMock.Verify(repo => repo.CreateMany(
            It.Is<DutchGame>(dg => dg.Comment == comment),
            It.Is<IEnumerable<DutchScore>>(scores =>
                scores.Count() == dutchGameToBeCreated.DutchScores.Count &&
                scores.All(
                    score => dutchGameToBeCreated.DutchScores.Any(
                        expected => expected.Points == score.Points &&
                                    expected.Position == score.Position)) &&
                dutchGameToBeCreated.DutchScores.All(
                    g => scores.Any(
                        expected => expected.Points == g.Points &&
                                    expected.Position == g.Position))),
            _token), Times.Once);
    }

    [Fact]
    public async Task CreateMany_ShouldThrowException_WhenSomeUsersAreMissing()
    {
        // Arrange
        var command = new CreateDutchGameCommand
        {
            Comment = "New Game",
            ScorePairs =
            [
                new() { NickName = "User A", Score = 50 },
                new() { NickName = "User B", Score = 100 }
            ]
        };
        
        var users = new List<AllStarUser>
        {
            new() { Id = Guid.NewGuid(), Nickname = "User A" }
        };

        _userRepositoryMock
            .Setup(repo => repo.GetManyAsync(It.IsAny<IEnumerable<string>>(), _token))
            .ReturnsAsync(users);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _dutchService.CreateMany(command, _token));
    }


}
