using Microsoft.AspNetCore.Mvc;
using Library.DTO.Auth;
using Library.Services.Auth;

namespace Library.Controllers.Auth
{
    [ApiController]
    [Route("api/auth/register")]

    public class RegisterController : ControllerBase
    {
        private readonly IRegisterServices _register;

        public RegisterController(IRegisterServices register)
        {
            _register = register;
        }

        [HttpPost]
        public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request,  CancellationToken ct)
        {
            var resp = await _register.Register(request, ct);
            if (resp is null) return Unauthorized(new { message = "Invalid credentials" });
            return Ok(resp);
        }
    }
    
    
}