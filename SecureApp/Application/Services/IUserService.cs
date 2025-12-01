using Shared.Domain.Models;

namespace SecureApp.Application.Services;

// SECURE: Service interface for business logic abstraction
public interface IUserService
{
    Task<User?> AuthenticateAsync(LoginRequest request);
    Task<int> CreateUserAsync(CreateUserRequest request);
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserAsync(int id);
}

public interface ITokenService
{
    string GenerateToken(User user);
    bool ValidateToken(string token);
}