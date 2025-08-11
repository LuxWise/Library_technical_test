using System.Text.RegularExpressions;
using LogWorker.Logs;
using Microsoft.Extensions.Options;
using Metrics.Data;
using Metrics.Model;
using Microsoft.EntityFrameworkCore;

namespace LogWorker.Services
{
    public class LogIngestionBackgroundService : BackgroundService
{
    private readonly ILogger<LogIngestionBackgroundService> _logger;
    private readonly IServiceProvider _sp;
    private readonly LogOptions _opts;

    private static readonly Regex LineRegex = new(
        @"^(?<date>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}) \[(?<level>INFO|ERROR|WARN)\] (?<message>.*)$",
        RegexOptions.Compiled);

    public LogIngestionBackgroundService(
        ILogger<LogIngestionBackgroundService> logger,
        IServiceProvider sp,
        IOptions<LogOptions> opts)
    {
        _logger = logger;
        _sp = sp;
        _opts = opts.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Directory.CreateDirectory(_opts.WatchPath);

        foreach (var file in Directory.GetFiles(_opts.WatchPath, _opts.Pattern))
            await ProcessFileSafe(file, stoppingToken);

        if (_opts.UseFileSystemWatcher)
        {
            using var fsw = new FileSystemWatcher(_opts.WatchPath, _opts.Pattern)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = false
            };
            fsw.Created += async (_, e) => await ProcessFileSafe(e.FullPath, stoppingToken);

            _logger.LogInformation("Watching {Path} for {Pattern}", _opts.WatchPath, _opts.Pattern);
            
            foreach (var file in Directory.GetFiles(_opts.WatchPath, _opts.Pattern))
                await ProcessFileSafe(file, stoppingToken);
            
            while (!stoppingToken.IsCancellationRequested)
                await Task.Delay(1000, stoppingToken);
        }
        else
        {
            _logger.LogInformation("Polling {Path}...", _opts.WatchPath);
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var file in Directory.GetFiles(_opts.WatchPath, _opts.Pattern))
                    await ProcessFileSafe(file, stoppingToken);

                await Task.Delay(3000, stoppingToken);
            }
        }
    }

    private async Task ProcessFileSafe(string path, CancellationToken ct)
    {
        try
        {
            await ProcessFile(path, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing file {File}", path);

            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MetricsDbContext>();
            var name = Path.GetFileName(path);
            var lf = await db.LogFile.FirstOrDefaultAsync(f => f.FileName == name, ct);
            if (lf != null)
            {
                lf.Status = "Failed";
                await db.SaveChangesAsync(ct);
            }
        }
    }

    private async Task ProcessFile(string path, CancellationToken ct)
    {
        var fileName = Path.GetFileName(path);

        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MetricsDbContext>();

        var hash = await ComputeHash(path, ct);
        var existing = await db.LogFile
            .FirstOrDefaultAsync(x => x.FileName == fileName && x.Hash == hash, ct);
        if (existing != null)
        {
            _logger.LogInformation("Skip {File} (already processed)", fileName);
            return;
        }

        var logFile = new LogFile()
        {
            FileName = fileName,
            Hash = hash,
            ProcessedAt = DateTime.UtcNow,
            Status = "Processed"
        };

        // Lee y parsea líneas
        var lines = await File.ReadAllLinesAsync(path, ct);
        foreach (var line in lines)
        {
            var m = LineRegex.Match(line);
            if (!m.Success) continue;

            var ts  = DateTime.ParseExact(m.Groups["date"].Value, "yyyy-MM-dd HH:mm:ss", null);
            var lvl = m.Groups["level"].Value;
            var msg = m.Groups["message"].Value;

            logFile.Entries.Add(new LogEntry
            {
                Timestamp = ts,
                Level = lvl,
                Message = msg
            });
        }

        db.LogFile.Add(logFile);
        await db.SaveChangesAsync(ct);

        _logger.LogInformation("Processed {Count} entries from {File}", logFile.Entries.Count, fileName);
    }

    private static async Task<string> ComputeHash(string path, CancellationToken ct)
    {
        await using var stream = File.OpenRead(path);
        using var sha = System.Security.Cryptography.SHA256.Create();
        var hash = await sha.ComputeHashAsync(stream, ct);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}

}

