using Library.Services.Metric;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers.Metric
{
    [ApiController]
    [Route("api/metrics")]
    public class MetricController : ControllerBase
    {
        private readonly IMetricServices _metricServices;
           
        public MetricController(IMetricServices metricServices)
        {
            _metricServices = metricServices;
        }
        
        /// <summary>
        /// Count by level (INFO/WARN/ERROR) in date range.
        /// </summary>
        [HttpGet("levels")]
        [Authorize]
        public async Task<IActionResult> GetLevels([FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken ct = default)
        {
            var result = await _metricServices.GetLevels(from, to, ct);
            return Ok(result);
        }
        
        /// <summary>
        /// Hourly or daily time series.
        /// </summary>
        [HttpGet("timeseries")]
        [Authorize]
        public async Task<IActionResult> GetTimeSeries([FromQuery] string granularity, [FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken ct = default)
        {
            var result = await _metricServices.GetTimeSeries(granularity, from, to, ct);
            return Ok(result);
        }
        
        /// <summary>
        /// Processed files with their status, date and number of entries.
        /// </summary>
        [HttpGet("files")]
        [Authorize]
        public async Task<IActionResult> GetFiles([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _metricServices.GetFiles(page, pageSize, ct);
            return Ok(result);
        }
    }    
}