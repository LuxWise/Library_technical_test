using Library.Data;
using Library.DTO.Auth;
using Library.Model;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Library.Services.Auth
{
    public class RegisterServices : IRegisterServices
    {
        private readonly LibraryDbContext _db;
        public RegisterServices(LibraryDbContext db)
        {
            _db = db;
        }
        
        public async Task<RegisterResponse?> Register(RegisterRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Email and Password are required.");

            var email = request.Email.Trim().ToLowerInvariant();
            var name  = request.Name?.Trim() ?? string.Empty;
         
            var exists = await _db.User.AsNoTracking().AnyAsync(u => u.Email == email, ct);
            if (exists)
                throw new InvalidOperationException("User already exists");
            
            var user = new User
            {
                Name = name,
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _db.User.Add(user);
            
            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (ex.InnerException is MySqlException mysql && mysql.Number == 1062)
            {
                throw new InvalidOperationException("User already exists");
            }
            
            return new RegisterResponse{message = "User registered successfully!"};
        }
    }    
}
