namespace GameHub.Events;

public class TelemetryEvent
{
    public string EventType { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Details { get; set; } = string.Empty;
}