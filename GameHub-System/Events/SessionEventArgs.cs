namespace GameHub.Events;

public class SessionEventArgs : EventArgs
{
    public int UserId { get; set; }
    public int GameId { get; set; }
    public DateTime Time { get; set; } = DateTime.Now;
}