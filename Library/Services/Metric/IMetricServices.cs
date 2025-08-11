namespace Library.Services.Metric
{

    public interface IMetricServices
    {
        Task<object> GetLevels(DateTime from, DateTime to, CancellationToken ct = default);
        Task<object> GetTimeSeries(string granularity, DateTime from, DateTime to, CancellationToken ct = default);
        Task<object> GetFiles(int page, int pageSize, CancellationToken ct = default);
    }    
}
