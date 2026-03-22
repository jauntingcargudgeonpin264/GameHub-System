using GameHub.Models;
using GameHub.Services;

namespace GameHub.Tests;

public class GameHubServiceTests
{
    private GameHubService CreateHub()
    {
        var tmpPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        return new GameHubService(tmpPath);
    }
    [Fact]
    public void AddGame_ShouldStoreGame()
    {
        var hub = CreateHub();
        var game = new Game { Id = 1, Title = "Elden Ring", Genre = GameGenre.Action, Price = 59.99m };

        hub.AddGame(game);

        var result = hub.GetGameById(1);
        Assert.NotNull(result);
        Assert.Equal("Elden Ring", result.Title);
    }

    [Fact]
    public void AddUser_ShouldStoreUser()
    {
        var hub = CreateHub();
        var user = new User { Id = 1, Name = "Alice" };

        hub.AddUser(user);
        var top = hub.TopUsersByPoints(1);

        Assert.Empty(top);
    }

    [Fact]
    public void StartSession_And_EndSession_ShouldCreateCompletedSession()
    {
        var hub = CreateHub();
        hub.AddGame(new Game { Id = 1, Title = "Elden Ring", Genre = GameGenre.Action });
        hub.AddUser(new User { Id = 1, Name = "Alice" });

        hub.StartSession(1, 1);
        Thread.Sleep(100);
        hub.EndSession(1, 1);

        var sessions = hub.GetUserSessions(1);
        Assert.Single(sessions);
        Assert.NotEqual(default, sessions[0].EndTime);
    }

    [Fact]
    public void TopUsersByPoints_ShouldReturnUsersOrderedByPoints()
    {
        var hub = CreateHub();
        hub.AddUser(new User { Id = 1, Name = "Alice" });
        hub.AddUser(new User { Id = 2, Name = "Bob" });
        hub.AddAchievement(new Achievement { Code = "ACH1", Name = "Test", Points = 50 });
        hub.AddAchievement(new Achievement { Code = "ACH2", Name = "Test2", Points = 10 });

        hub.UnlockAchievement(1, "ACH1");
        hub.UnlockAchievement(2, "ACH2");

        var top = hub.TopUsersByPoints(2);
        Assert.Equal("Alice", top[0].Name);
        Assert.Equal("Bob", top[1].Name);
    }

    [Fact]
    public void AchievementsNotUnlocked_ShouldReturnOnlyLocked()
    {
        var hub = CreateHub();
        hub.AddUser(new User { Id = 1, Name = "Alice" });
        hub.AddAchievement(new Achievement { Code = "ACH1", Name = "First", Points = 10 });
        hub.AddAchievement(new Achievement { Code = "ACH2", Name = "Second", Points = 20 });

        hub.UnlockAchievement(1, "ACH1");

        var notUnlocked = hub.AchievementsNotUnlocked(1);
        Assert.Single(notUnlocked);
        Assert.Equal("ACH2", notUnlocked[0].Code);
    }

    [Fact]
    public void HasUserUnlockedAchievement_ShouldReturnTrueAfterUnlock()
    {
        var hub = CreateHub();
        hub.AddUser(new User { Id = 1, Name = "Alice" });
        hub.AddAchievement(new Achievement { Code = "ACH1", Name = "First", Points = 10 });

        hub.UnlockAchievement(1, "ACH1");

        Assert.True(hub.HasUserUnlockedAchievement(1, "ACH1"));
    }

    [Fact]
    public void UnlockAchievement_ShouldNotDuplicate()
    {
        var hub = CreateHub();
        hub.AddUser(new User { Id = 1, Name = "Alice" });
        hub.AddAchievement(new Achievement { Code = "ACH1", Name = "First", Points = 10 });

        hub.UnlockAchievement(1, "ACH1");
        hub.UnlockAchievement(1, "ACH1");

        var notUnlocked = hub.AchievementsNotUnlocked(1);
        Assert.Empty(notUnlocked);
    }

    [Fact]
    public async Task SaveAsync_And_LoadAsync_ShouldPersistData()
    {
        var tmpPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var hub = new GameHubService(tmpPath);
        hub.AddGame(new Game { Id = 1, Title = "Elden Ring", Genre = GameGenre.Action });
        hub.AddUser(new User { Id = 1, Name = "Alice" });

        await hub.SaveAsync(tmpPath);

        var hub2 = new GameHubService(tmpPath);
        await hub2.LoadAsync(tmpPath);

        Assert.NotNull(hub2.GetGameById(1));
    }
}