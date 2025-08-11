using Library.Exeptions;
using Metrics.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Services.Metric
{

    public class MetricServices: IMetricServices
    {
        private readonly MetricsDbContext _db;

        public MetricServices(MetricsDbContext db)
        {
            _db = db;
        }
        
        public async Task<object> GetLevels(DateTime from, DateTime to, CancellationToken ct = default)
        {
            var q = _db.LogEntry
                .Where(x => x.Timestamp >= from && x.Timestamp <= to);

            var total = await q.CountAsync(ct);
            var info = await q.CountAsync(x => x.Level == "INFO", ct);
            var warn = await q.CountAsync(x => x.Level == "WARN", ct);
            var error = await q.CountAsync(x => x.Level == "ERROR", ct);
            
            return new
            {
                Total = total,
                Info = info,
                Warn = warn,
                Error = error
            };
        }
        
        public async Task<object> GetTimeSeries(string granularity, DateTime from, DateTime to, CancellationToken ct = default)
        {

            if (granularity != "hour" && granularity != "day")
                throw new NotFoundException("granularity debe ser 'hour' o 'day'.");

            var rows = await _db.LogEntry
                .Where(x => x.Timestamp >= from && x.Timestamp <= to)
                .Select(x => new { x.Timestamp, x.Level })
                .ToListAsync(ct);

            var groups = rows
                .GroupBy(x =>
                {
                    var ts = x.Timestamp;
                    return granularity == "hour"
                        ? new DateTime(ts.Year, ts.Month, ts.Day, ts.Hour, 0, 0, DateTimeKind.Utc)
                        : new DateTime(ts.Year, ts.Month, ts.Day, 0, 0, 0, DateTimeKind.Utc);
                })
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    timestamp = g.Key,
                    info = g.Count(r => r.Level == "INFO"),
                    warn = g.Count(r => r.Level == "WARN"),
                    error = g.Count(r => r.Level == "ERROR"),
                    total = g.Count()
                });
            return groups;
        }
        
        public async Task<object> GetFiles(int page, int pageSize, CancellationToken ct = default)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

            var q = _db.LogFile
                .AsNoTracking()
                .OrderByDescending(x => x.ProcessedAt)
                .Select(x => new
                {
                    x.Id,
                    x.FileName,
                    x.Hash,
                    x.Status,
                    x.ProcessedAt,
                    entries = x.Entries.Count
                });

            var total = await q.CountAsync(ct);
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            
            return new
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = items
            };
        }
    }    
}
