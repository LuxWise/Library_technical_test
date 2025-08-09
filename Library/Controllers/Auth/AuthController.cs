using Microsoft.AspNetCore.Mvc;
using Library.Data;
using Library.DTO.Auth;

namespace Library.Controllers.Auth
{
	[ApiController]
	[Route("api/auth")]

	public class AuthController : ControllerBase 
	{
		private readonly LibraryDbContext _context;
		public AuthController(LibraryDbContext context) 
		{
			_context = context;
		}

		[HttpPost("login")]
		public LoginResponse Login([FromBody] LoginRequest request)
		{
			return new LoginResponse
			{
				Token = "fasfea",
				Status = "success"
			};
		}
		

	}
}