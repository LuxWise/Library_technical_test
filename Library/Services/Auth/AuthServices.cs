using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Library.Data;
using Library.DTO.Auth;
using Library.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Library.Services.Auth
{

    public class AuthServices : IAuthService
    {

        private readonly LibraryDbContext _db;
        private readonly JwtOptions _jwt;

        public AuthServices(LibraryDbContext db, IOptions<JwtOptions> jwt)
        {
            _db = db;
            _jwt = jwt.Value;
        }

        public async Task<LoginResponse?> Login(LoginRequest request, CancellationToken ct = default)
        {
            var user = await _db.User
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == request.Email, ct);

            if (user is null)
                return null;

            var ok = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!ok) return null;

            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(_jwt.ExpirationMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Name, user.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponse
            {
                Token = jwt,
                Status = "success"
            };
        }
    }
    
}
