using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Library.Services.Auth.Users
{
    public class CurrentUserService: ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (Guid.TryParse(userId, out var id))
                return id;

            return null;

        }
        
        public string? GetUserEmail()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Email);
        }

        public string? GetUserName()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
        }
    }   
}