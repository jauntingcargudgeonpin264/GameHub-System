# 🎮 GameHub — Game Library · Achievements · Telemetry

[![.NET 10](https://img.shields.io/badge/.NET-10.0-blueviolet)](https://dotnet.microsoft.com/)
[![Language](https://img.shields.io/badge/language-C%23-239120?logo=csharp)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![Paradigm](https://img.shields.io/badge/paradigm-OOP%20%2B%20LINQ-blue)]()
[![Storage](https://img.shields.io/badge/persistence-JSON%20%2F%20System.Text.Json-orange)]()
[![License](https://img.shields.io/badge/license-MIT-green)]()

## 📖 Overview

**GameHub** is a feature-rich in-memory game library management system built entirely in C# (.NET 10). It models a real-world gaming platform with user tracking, play sessions, an achievement system driven by custom delegate-based rules, and a full telemetry pipeline — all persisted to and restored from structured JSON files.

The project demonstrates mastery of five core C# pillars:

| Pillar | What's covered |
|---|---|
| **Collections & Models** | Strongly typed domain entities, in-memory storage via `List<T>` |
| **LINQ** | Complex aggregations, groupings, joins, projections |
| **Events & Telemetry** | `EventHandler<TEventArgs>`, custom `EventArgs`, event log |
| **Delegates as Rules** | `delegate bool AchievementRule(...)`, pluggable rule engine |
| **JSON Persistence** | Full save/load with `System.Text.Json`, safe continuation of IDs |

## 🏗️ Architecture

```
GameHub/
├── models/
│   ├── Game.cs                  # Game entity (Id, Title, Genre, Price)
│   ├── User.cs                  # User entity (Id, Name)
│   ├── PlaySession.cs           # Session (UserId, GameId, Start, End)
│   ├── Achievement.cs           # Achievement (Code, Name, Points)
│   └── Unlock.cs                # Unlock record (UserId, Code, Time)
│
├── Service/
│   ├── GameHub.cs               # Core service — collections, LINQ, events, Save/Load
│   ├── AchievementEngine.cs     # Delegate-based rule engine
│   └── EventArgs/
│       ├── SessionEventArgs.cs          # UserId, GameId, Time
│       └── AchievementUnlockedEventArgs.cs  # UserId, Code, Points, Time
│
├── data/                        # JSON persistence folder
│   ├── games.json
│   ├── users.json
│   ├── sessions.json
│   ├── achievements.json
│   ├── unlocks.json
│   └── telemetry.json           # Full event log (Start / End / Unlock)
│
└── Program.cs                   # Entry point & demonstration scenarios
```

## 🛠️ Technology Stack

- **Runtime:** .NET 10.0
- **Language:** C# 13
- **Storage:** In-memory (`List<T>`, `Dictionary<K,V>`)
- **Serialization:** `System.Text.Json` with `WriteIndented = true`
- **Patterns:** Service Layer, Rule Engine (Delegate Pattern), Observer (Events)

## 📦 Domain Models

### `Game`
```csharp
public class Game
{
    public int    Id     { get; set; }
    public string Title  { get; set; }
    public string Genre  { get; set; }
    public decimal Price { get; set; }
}
```

### `User`
```csharp
public class User
{
    public int    Id   { get; set; }
    public string Name { get; set; }
}
```

### `PlaySession`
```csharp
public class PlaySession
{
    public int      UserId  { get; set; }
    public int      GameId  { get; set; }
    public DateTime Start   { get; set; }
    public DateTime End     { get; set; }   // DateTime.MinValue while active
}
```

### `Achievement`
```csharp
public class Achievement
{
    public string Code   { get; set; }
    public string Name   { get; set; }
    public int    Points { get; set; }
}
```

### `Unlock`
```csharp
public class Unlock
{
    public int      UserId          { get; set; }
    public string   AchievementCode { get; set; }
    public DateTime Time            { get; set; }
}
```

## ⚙️ GameHub Service

The central `GameHub` class acts as the in-memory data store and business logic engine.

### Collections
```csharp
public List<Game>        Games        { get; }
public List<User>        Users        { get; }
public List<PlaySession> Sessions     { get; }
public List<Achievement> Achievements { get; }
public List<Unlock>      Unlocks      { get; }
```

### Management Methods
| Method | Description |
|---|---|
| `AddGame(title, genre, price)` | Registers a new game with auto-incremented ID |
| `AddUser(name)` | Creates a new user with auto-incremented ID |
| `AddAchievement(code, name, points)` | Registers a new achievement |
| `StartSession(userId, gameId)` | Opens a new play session, raises `SessionStarted` |
| `EndSession(userId, gameId)` | Closes the last active session, raises `SessionEnded` |

## 🔍 LINQ Analytics

All four analytics methods are implemented **exclusively via LINQ** — no loops, no manual aggregation.

### 1. `TotalMinutesByGenre(int userId)` → `Dictionary<string, int>`
Groups completed sessions by game genre and sums total playtime in minutes for the specified user.

```csharp
// Example output:
// { "RPG" → 120, "FPS" → 45, "Strategy" → 300 }
```

### 2. `Top3GamesByPlayTime(int userId)` → `List<(Game, int minutes)>`
Joins sessions with games, aggregates total play time per game, and returns the top 3 ordered descending.

```csharp
// Example output:
// [ (Witcher 3, 180), (Minecraft, 95), (Cyberpunk 2077, 60) ]
```

### 3. `TopUsersByPoints(int topN)` → `List<User>`
Calculates each user's total achievement score via `Unlocks → Achievements`, then returns the top N users ranked by score.

```csharp
// Example: topN = 3 → returns 3 highest-scoring users
```

### 4. `AchievementsNotUnlocked(int userId)` → `List<Achievement>`
Returns all achievements the given user has **not yet** unlocked, using a set-exclusion pattern.

```csharp
// Useful for "locked achievements" display in UI
```

## 📡 Events & Telemetry

GameHub exposes three typed events forming the telemetry backbone of the system.

```csharp
public event EventHandler<SessionEventArgs>            SessionStarted;
public event EventHandler<SessionEventArgs>            SessionEnded;
public event EventHandler<AchievementUnlockedEventArgs> AchievementUnlocked;
```

### `SessionEventArgs`
```csharp
public class SessionEventArgs : EventArgs
{
    public int      UserId { get; init; }
    public int      GameId { get; init; }
    public DateTime Time   { get; init; }
}
```

### `AchievementUnlockedEventArgs`
```csharp
public class AchievementUnlockedEventArgs : EventArgs
{
    public int      UserId          { get; init; }
    public string   AchievementCode { get; init; }
    public int      Points          { get; init; }
    public DateTime Time            { get; init; }
}
```

All fired events are automatically serialized into `telemetry.json` as a chronological event log, enabling post-session analytics and audit trails.

## 🏆 Achievement Engine (Delegate Rules)

The `AchievementEngine` uses a pluggable, delegate-driven rule system to automatically evaluate and unlock achievements.

### The Rule Delegate
```csharp
public delegate bool AchievementRule(GameHub hub, int userId, out string reason);
```

### `AchievementEngine` API
```csharp
// Register a rule
engine.Register("FIRST_SESSION", (hub, userId, out reason) => { ... });

// Evaluate all rules for a user — unlocks matching, unearned achievements
engine.Evaluate(userId);
```

### Built-in Rules

| Code | Name | Condition |
|---|---|---|
| `FIRST_SESSION` | First Blood | User has completed at least **1** play session |
| `HOUR_TOTAL` | Marathon Gamer | Total playtime across all games **≥ 60 minutes** |
| `GENRE_FAN` | Genre Fanatic | **≥ 3 sessions** played in the same genre |

> Adding new achievements requires zero changes to `GameHub` — simply register a new `AchievementRule` delegate in `AchievementEngine`. The engine is fully open for extension.

## 💾 JSON Persistence

### `Save(string folderPath)`

Serializes the entire hub state to 6 JSON files. Uses `WriteIndented = true` for human-readable output.

```
data/
├── games.json
├── users.json
├── sessions.json
├── achievements.json
├── unlocks.json
└── telemetry.json    ← full chronological event log
```

### `Load(string folderPath)`

Restores all collections from disk. Guarantees:
- **No crash** if files are missing — starts fresh silently.
- **No ID collisions** — auto-increment counters are recalculated from loaded data (`Max(Id) + 1`).
- **Seamless continuation** — after loading, all operations (sessions, achievements, new entities) work identically to a fresh start.

```csharp
var hub = new GameHub();
hub.Load("data/");

// Continue exactly where you left off
hub.StartSession(userId: 1, gameId: 3);
```

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Run the Project

```bash
git clone https://github.com/korolslava/exam_cs_24_1.git
cd exam_cs_24_1
dotnet run
```

The `Program.cs` entry point demonstrates a full end-to-end scenario:
1. Creates games, users, and achievements
2. Simulates play sessions (start → end)
3. Runs the `AchievementEngine` to evaluate rules
4. Prints LINQ analytics to the console
5. Saves full state to `data/` folder
6. Reloads and continues with a new session to verify persistence

## 📊 Sample Output

```
[TELEMETRY] SessionStarted  → User 1 started 'The Witcher 3' at 14:00:00
[TELEMETRY] SessionEnded    → User 1 ended  'The Witcher 3' at 15:30:00 (90 min)
[ACHIEVEMENT] 🏆 FIRST_SESSION unlocked for User 1! (+10 pts)
[ACHIEVEMENT] 🏆 HOUR_TOTAL   unlocked for User 1! (+50 pts)

--- Top 3 Games by Playtime (User 1) ---
1. The Witcher 3   — 180 min
2. Cyberpunk 2077  — 95 min
3. Minecraft       — 60 min

--- Total Minutes by Genre (User 1) ---
RPG      → 275 min
Sandbox  → 60 min

--- Top 3 Users by Points ---
1. Slava  — 180 pts
2. Alex   — 110 pts
3. Maria  — 60 pts

[SAVE] State saved to /data (6 files)
[LOAD] State restored. Continuing...
```

## 🧠 Key Design Decisions

- **Delegate-based Rule Engine** — achievement logic is fully decoupled from the core service. New rules are registered at startup, not hardcoded into `GameHub`.
- **Event-driven Telemetry** — all significant state changes fire typed events. The telemetry log is a side-effect of the event system, not manually maintained.
- **LINQ-only Analytics** — all four query methods use pure LINQ chains with no imperative loops, showcasing expressive, declarative query composition.
- **Safe Persistence** — the Save/Load cycle is idempotent: load → work → save → load again always produces a consistent, non-duplicated state.
