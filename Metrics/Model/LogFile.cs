namespace Metrics.Model;

public class LogFile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = default!;
    public string? Hash { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string Status { get; set; } = "Processed"; // Processed|Failed|Pending
    public ICollection<LogEntry> Entries { get; set; } = new List<LogEntry>();
}