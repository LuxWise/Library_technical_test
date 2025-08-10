using Microsoft.AspNetCore.Mvc;
using Library.DTO.Auth;
using Library.Services.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Library.Controllers.Auth
{
	[ApiController]
	[Route("api/auth")]

	public class AuthController : ControllerBase 
	{
		private readonly IAuthService _auth;
		public AuthController(IAuthService auth) 
		{
			_auth = auth;
		}

		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
		{
			var resp = await _auth.Login(request, ct);
			if (resp is null) return Unauthorized(new { message = "Invalid credentials" });
			return Ok(resp);
		}
		

	}
}