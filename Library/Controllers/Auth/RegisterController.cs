using Microsoft.AspNetCore.Mvc;
using Library.Data;
using Library.DTO.Auth;

namespace Library.Controllers.Auth
{
    [ApiController]
    [Route("api/auth/register")]

    public class RegisterController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public RegisterController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public RegisterResponse Register([FromBody] RegisterRequest request)
        {
            return new RegisterResponse
            {
                message = "User registered successfully"
            };
        }
    }
    
    
}