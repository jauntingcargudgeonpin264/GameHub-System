using GameHub.Models;

namespace GameHub.Events;

public class AchievementUnlockedEventArgs : EventArgs
{
    public int UserId { get; set; }
    public string AchievementCode { get; set; } = string.Empty;
    public Achievement? Achievement { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime Time { get; set; } = DateTime.Now;
}