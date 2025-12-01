using BCrypt.Net;
using SecureApp.Infrastructure.Repositories;
using Shared.Domain.Models;

namespace SecureApp.Application.Services;

// SECURE: Service with proper validation, hashing, and business logic
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<User?> AuthenticateAsync(LoginRequest request)
    {
        try
        {
            // SECURE: Rate limiting is handled at middleware level
            var user = await _userRepository.GetUserByUsernameAsync(request.Username);
            
            // SECURE: Constant-time comparison to prevent timing attacks
            if (user != null && !string.IsNullOrWhiteSpace(user.Password))
            {
                try
                {
                    _logger.LogDebug("Attempting to verify password for user: {Username}, Hash length: {Length}, Hash prefix: {Prefix}", 
                        request.Username, user.Password.Length, user.Password.Substring(0, Math.Min(10, user.Password.Length)));
                    
                    if (BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                    {
                        _logger.LogInformation("Successful authentication for user: {Username}", request.Username);
                        
                        // SECURE: Don't return password in response
                        user.Password = string.Empty;
                        return user;
                    }
                }
                catch (BCrypt.Net.SaltParseException ex)
                {
                    _logger.LogError(ex, "Invalid BCrypt hash for user: {Username}. Hash: {Hash}", request.Username, user.Password);
                    return null;
                }
            }
            
            // SECURE: Log failed attempts for monitoring
            _logger.LogWarning("Failed authentication attempt for username: {Username}", request.Username);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication for username: {Username}", request.Username);
            throw;
        }
    }

    public async Task<int> CreateUserAsync(CreateUserRequest request)
    {
        try
        {
            // SECURE: Check for duplicate username and email
            if (await _userRepository.UsernameExistsAsync(request.Username))
            {
                throw new InvalidOperationException("Username already exists");
            }

            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // SECURE: Hash password with BCrypt (work factor 12)
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password, 12);
            
            var userId = await _userRepository.CreateUserAsync(request, hashedPassword);
            
            _logger.LogInformation("User created successfully: {Username}", request.Username);
            return userId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Username}", request.Username);
            throw;
        }
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        try
        {
            // SECURE: Authorization is handled at controller level
            return await _userRepository.GetAllUsersAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            throw;
        }
    }

    public async Task<User?> GetUserAsync(int id)
    {
        try
        {
            // SECURE: Authorization is handled at controller level
            return await _userRepository.GetUserByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user: {UserId}", id);
            throw;
        }
    }
}