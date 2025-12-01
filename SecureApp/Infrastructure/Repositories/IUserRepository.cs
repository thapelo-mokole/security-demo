using Shared.Domain.Models;

namespace SecureApp.Infrastructure.Repositories;

// SECURE: Interface for dependency injection and testability
public interface IUserRepository
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByIdAsync(int id);
    Task<int> CreateUserAsync(CreateUserRequest request, string hashedPassword);
    Task<List<User>> GetAllUsersAsync();
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
}