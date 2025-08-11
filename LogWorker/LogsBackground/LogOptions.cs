namespace LogWorker.Logs;

public class LogOptions
{
    public string WatchPath { get; set; } = "/logs";
    public string Pattern { get; set; } = "*.log";
    public int BatchSize { get; set; } = 500;
    public bool UseFileSystemWatcher { get; set; } = true;
}