using Library.DTO.Auth;

namespace Library.Services.Auth
{
    public interface IAuthService
    {
    
        Task<LoginResponse?> Login(LoginRequest request, CancellationToken ct = default);

    }    
}

