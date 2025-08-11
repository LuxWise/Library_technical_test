namespace Metrics.Model;

public class LogEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = default!; // INFO|ERROR|WARN
    public string Message { get; set; } = default!;
    public Guid LogFileId { get; set; }
    public LogFile LogFile { get; set; } = default!;
}
