using System.Text.Json;

using GameHub.Models;
using GameHub.Events;
namespace GameHub.Services;

public class GameHubService
{
    public delegate bool AchievementRule(GameHubService hub, int userId, out string reason);

    List<Game> _games;
    List<User> _users;
    List<PlaySession> _playSessions;
    List<Achievement> _achievements;
    List<Unlock> _unlocks;
    List<TelemetryEvent> _telemetry;

    string _dataFolderPath;

    const string FileNameGames = "games.json";
    const string FileNameUsers = "users.json";
    const string FileNamePlaySessions = "playSessions.json";
    const string FileNameAchievements = "achievements.json";
    const string FileNameUnlocks = "unlocks.json";
    const string FileNameTelemetry = "telemetry.json";

    private static readonly JsonSerializerOptions WriteOptions = new() { WriteIndented = true };
    private static readonly JsonSerializerOptions ReadOptions = new() { PropertyNameCaseInsensitive = true };

    public event EventHandler<SessionEventArgs> SessionStarted;
    public event EventHandler<SessionEventArgs> SessionEnded;
    public event EventHandler<AchievementUnlockedEventArgs> AchievementUnlocked;

    public GameHubService(string dataFolderPath = "data")
    {
        _dataFolderPath = dataFolderPath;
        if (!Directory.Exists(_dataFolderPath))
        {
            Directory.CreateDirectory(_dataFolderPath);
        }

        _games = new List<Game>();
        _users = new List<User>();
        _playSessions = new List<PlaySession>();
        _achievements = new List<Achievement>();
        _unlocks = new List<Unlock>();
        _telemetry = new List<TelemetryEvent>();
    }

    public void Save(string folderPath)
    {
        _dataFolderPath = folderPath;
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        void WriteJsonFile(string filePath, string jsonContent)
        {
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            using var writer = new StreamWriter(fileStream);
            writer.Write(jsonContent);
        }

        WriteJsonFile(Path.Combine(folderPath, FileNameGames), JsonSerializer.Serialize(_games, WriteOptions));
        WriteJsonFile(Path.Combine(folderPath, FileNameUsers), JsonSerializer.Serialize(_users, WriteOptions));
        WriteJsonFile(Path.Combine(folderPath, FileNamePlaySessions), JsonSerializer.Serialize(_playSessions, WriteOptions));
        WriteJsonFile(Path.Combine(folderPath, FileNameAchievements), JsonSerializer.Serialize(_achievements, WriteOptions));
        WriteJsonFile(Path.Combine(folderPath, FileNameUnlocks), JsonSerializer.Serialize(_unlocks, WriteOptions));
        WriteJsonFile(Path.Combine(folderPath, FileNameTelemetry), JsonSerializer.Serialize(_telemetry, WriteOptions));
    }

    public void Load(string folderPath)
    {
        _dataFolderPath = folderPath;
        if (!Directory.Exists(folderPath)) { Directory.CreateDirectory(folderPath); return; }

        T? ReadJson<T>(string fileName) where T : class
        {
            var path = Path.Combine(folderPath, fileName);
            return File.Exists(path)
                ? JsonSerializer.Deserialize<T>(File.ReadAllText(path), ReadOptions)
                : null;
        }

        _games = ReadJson<List<Game>>(FileNameGames) ?? new();
        _users = ReadJson<List<User>>(FileNameUsers) ?? new();
        _playSessions = ReadJson<List<PlaySession>>(FileNamePlaySessions) ?? new();
        _achievements = ReadJson<List<Achievement>>(FileNameAchievements) ?? new();
        _unlocks = ReadJson<List<Unlock>>(FileNameUnlocks) ?? new();
        _telemetry = ReadJson<List<TelemetryEvent>>(FileNameTelemetry) ?? new();
    }

    private void LogTelemetry(string eventType, int userId, string details = "")
    {
        _telemetry.Add(new TelemetryEvent
        {
            EventType = eventType,
            UserId = userId,
            Timestamp = DateTime.Now,
            Details = details
        });
    }

    public void AddGame(Game game)
    {
        _games.Add(game);
        var jsonString = JsonSerializer.Serialize(_games, WriteOptions);
        var filePath = Path.Combine(_dataFolderPath, FileNameGames);
        File.WriteAllText(filePath, jsonString);
    }

    public void AddUser(User user)
    {
        _users.Add(user);
        var jsonString = JsonSerializer.Serialize(_users, WriteOptions);
        var filePath = Path.Combine(_dataFolderPath, FileNameUsers);
        File.WriteAllText(filePath, jsonString);
    }

    public void AddAchievement(Achievement achievement)
    {
        _achievements.Add(achievement);
        var jsonString = JsonSerializer.Serialize(_achievements, WriteOptions);
        var filePath = Path.Combine(_dataFolderPath, FileNameAchievements);
        File.WriteAllText(filePath, jsonString);
    }

    public void AddUnlock(Unlock unlock)
    {
        _unlocks.Add(unlock);
        var jsonString = JsonSerializer.Serialize(_unlocks, WriteOptions);
        var filePath = Path.Combine(_dataFolderPath, FileNameUnlocks);
        File.WriteAllText(filePath, jsonString);
        LogTelemetry("Unlock", unlock.UserId, unlock.AchievementCode);
    }

    public void StartSession(int userId, int gameId)
    {
        _playSessions.Add(new PlaySession { UserId = userId, GameId = gameId, StartTime = DateTime.Now });
        SessionStarted?.Invoke(this, new SessionEventArgs { UserId = userId, GameId = gameId });
        LogTelemetry("Start", userId, $"GameId: {gameId}");
        var jsonString = JsonSerializer.Serialize(_playSessions, WriteOptions);
        var filePath = Path.Combine(_dataFolderPath, FileNamePlaySessions);
        File.WriteAllText(filePath, jsonString);
    }

    public void EndSession(int userId, int gameId)
    {
        var session = _playSessions.FindLast(s => s.UserId == userId && s.GameId == gameId && s.EndTime == default);
        if (session is null) return;

        session.EndTime = DateTime.Now;
        SessionEnded?.Invoke(this, new SessionEventArgs { UserId = userId, GameId = gameId });
        LogTelemetry("End", userId, $"GameId: {gameId}");
        var jsonString = JsonSerializer.Serialize(_playSessions, WriteOptions);
        var filePath = Path.Combine(_dataFolderPath, FileNamePlaySessions);
        File.WriteAllText(filePath, jsonString);
    }

    public Dictionary<string, int> TotalMinutesByGenre(int userId)
    {
        var genreMinutes = new Dictionary<string, int>();
        var userSessions = _playSessions.Where(s => s.UserId == userId && s.EndTime != default);

        foreach (var session in userSessions)
        {
            var game = _games.Find(g => g.Id == session.GameId);
            if (game != null)
            {
                var duration = (session.EndTime - session.StartTime).TotalMinutes;
                var genreKey = game.Genre.ToString();
                if (genreMinutes.ContainsKey(genreKey))
                {
                    genreMinutes[genreKey] += (int)duration;
                }
                else
                {
                    genreMinutes[genreKey] = (int)duration;
                }
            }
        }

        return genreMinutes;
    }

    public List<(Game game, int minutes)> Top3GamesByPlayTime(int userId)
    {
        var gameMinutes = new Dictionary<int, int>();
        var userSessions = _playSessions.Where(s => s.UserId == userId && s.EndTime != default);

        foreach (var session in userSessions)
        {
            var duration = (session.EndTime - session.StartTime).TotalMinutes;
            if (gameMinutes.ContainsKey(session.GameId))
            {
                gameMinutes[session.GameId] += (int)duration;
            }
            else
            {
                gameMinutes[session.GameId] = (int)duration;
            }
        }

        var top3Games = gameMinutes.OrderByDescending(gm => gm.Value).Take(3)
            .Select(gm =>
            {
                var game = _games.Find(g => g.Id == gm.Key);
                return (game!, gm.Value);
            }).ToList();

        return top3Games;
    }

    public List<User> TopUsersByPoints(int topN)
    {
        var userPoints = new Dictionary<int, int>();

        foreach (var unlock in _unlocks)
        {
            var achievement = _achievements.Find(a => a.Code == unlock.AchievementCode);
            if (achievement != null)
            {
                if (userPoints.ContainsKey(unlock.UserId))
                {
                    userPoints[unlock.UserId] += achievement.Points;
                }
                else
                {
                    userPoints[unlock.UserId] = achievement.Points;
                }
            }
        }

        var topUsers = userPoints.OrderByDescending(up => up.Value).Take(topN)
            .Select(up => _users.Find(u => u.Id == up.Key)!)
            .ToList();

        return topUsers;
    }

    public List<Achievement> AchievementsNotUnlocked(int userId)
    {
        var unlockedCodes = _unlocks.Where(u => u.UserId == userId).Select(u => u.AchievementCode).ToHashSet();
        var notUnlocked = _achievements.Where(a => !unlockedCodes.Contains(a.Code)).ToList();
        return notUnlocked;
    }

    public Achievement? GetAchievementByCode(string code)
    {
        return _achievements.Find(a => a.Code == code);
    }

    public bool HasUserUnlockedAchievement(int userId, string achievementCode)
    {
        return _unlocks.Any(u => u.UserId == userId && u.AchievementCode == achievementCode);
    }

    public void UnlockAchievement(int userId, string achievementCode)
    {
        if (!HasUserUnlockedAchievement(userId, achievementCode))
        {
            var unlock = new Unlock
            {
                UserId = userId,
                AchievementCode = achievementCode,
                UnlockDate = DateTime.Now
            };
            AddUnlock(unlock);
        }
    }

    public List<PlaySession> GetUserSessions(int userId)
    {
        return _playSessions.Where(s => s.UserId == userId && s.EndTime != default).ToList();
    }

    public Game? GetGameById(int gameId)
    {
        return _games.Find(g => g.Id == gameId);
    }

    internal void RaiseAchievementUnlocked(object sender, AchievementUnlockedEventArgs args)
    {
        AchievementUnlocked?.Invoke(sender, args);
    }
}