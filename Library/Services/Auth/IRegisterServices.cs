using Library.DTO.Auth;
using RegisterRequest = Library.DTO.Auth.RegisterRequest;

namespace Library.Services.Auth
{
    public interface IRegisterServices
    {
        Task<RegisterResponse?> Register(RegisterRequest request, CancellationToken ct = default);
    }    
}

