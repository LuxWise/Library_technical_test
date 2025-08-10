using Library.Services.Suggestions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers.Suggestions
{
    [ApiController]
    [Route("api/suggestions")]
    public class SuggestionsController : ControllerBase
    {
        private readonly ISuggestionsService _svc;

        public SuggestionsController(ISuggestionsService svc)
        {
            _svc = svc;
        }
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] int top = 10, CancellationToken ct = default)
        {
            var items = await _svc.SuggestionsAsync(top, ct);
            return Ok(items);
        }
    }    
}

