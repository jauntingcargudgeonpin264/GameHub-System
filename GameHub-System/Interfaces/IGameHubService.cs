using GameHub.Models;

namespace GameHub.Interfaces;

public interface IGameHubService
{
    void AddGame(Game game);
    void AddUser(User user);
    void AddAchievement(Achievement achievement);
    void StartSession(int userId, int gameId);
    void EndSession(int userId, int gameId);

    Task SaveAsync(string folderPath);
    Task LoadAsync(string folderPath);

    List<User> TopUsersByPoints(int topN);
    List<(Game game, int minutes)> Top3GamesByPlayTime(int userId);
    List<Achievement> AchievementsNotUnlocked(int userId);
}