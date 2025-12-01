using InsecureApp.Data;
using Shared.Domain.Models;

namespace InsecureApp.Services;

public class UserService
{
    private readonly UserRepository _userRepository = new(); // SECURITY RISK: Direct instantiation

    public async Task<User?> AuthenticateAsync(LoginRequest request)
    {
        // SECURITY RISK: No input validation
        var user = await _userRepository.GetUserByUsernameAsync(request.Username);
        
        // SECURITY RISK: Plaintext password comparison, timing attack vulnerability
        if (user != null && user.Password == request.Password)
        {
            return user;
        }
        return null;
    }

    public async Task<int> CreateUserAsync(CreateUserRequest request)
    {
        // SECURITY RISK: No input validation, duplicate checking, or password strength requirements
        return await _userRepository.CreateUserAsync(request);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        // SECURITY RISK: No authorization check - anyone can access all users
        return await _userRepository.GetAllUsersAsync();
    }

    public async Task<User?> GetUserAsync(string username)
    {
        // SECURITY RISK: No authorization - users can access other users' data
        return await _userRepository.GetUserByUsernameAsync(username);
    }
}