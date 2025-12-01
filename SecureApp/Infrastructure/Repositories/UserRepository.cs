using Microsoft.Data.SqlClient;
using Shared.Domain.Models;

namespace SecureApp.Infrastructure.Repositories;

// SECURE: Repository with parameterized queries and secure practices
public class UserRepository : IUserRepository
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IConfiguration configuration, ILogger<UserRepository> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    // SECURE: Connection string from configuration/secrets
    private string ConnectionString => _configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("Connection string not configured");

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        // SECURE: Parameterized query prevents SQL injection
        const string query = "SELECT Id, Username, Email, Password, Role, CreatedAt, IsActive FROM Users WHERE Username = @Username AND IsActive = 1";
        
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("Username", username);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3), // Still needed for password verification
                    Role = reader.GetString(4),
                    CreatedAt = reader.GetDateTime(5),
                    IsActive = reader.GetBoolean(6)
                };
            }
        }
        catch (Exception ex)
        {
            // SECURE: Log error without exposing sensitive details
            _logger.LogError(ex, "Error retrieving user by username");
            throw;
        }
        
        return null;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        const string query = "SELECT Id, Username, Email, Role, CreatedAt, IsActive FROM Users WHERE Id = @Id AND IsActive = 1";
        
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("Id", id);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    Role = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4),
                    IsActive = reader.GetBoolean(5)
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by ID: {UserId}", id);
            throw;
        }
        
        return null;
    }

    public async Task<int> CreateUserAsync(CreateUserRequest request, string hashedPassword)
    {
        // SECURE: Parameterized query with hashed password
        const string query = @"INSERT INTO Users (Username, Email, Password, Role, CreatedAt, IsActive) 
                              VALUES (@Username, @Email, @Password, @Role, GETDATE(), 1);
                              SELECT SCOPE_IDENTITY();";
        
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            using var command = new SqlCommand(query, connection);
            
            command.Parameters.AddWithValue("Username", request.Username);
            command.Parameters.AddWithValue("Email", request.Email);
            command.Parameters.AddWithValue("Password", hashedPassword);
            command.Parameters.AddWithValue("Role", request.Role);
            
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            
            _logger.LogInformation("User created successfully: {Username}", request.Username);
            return Convert.ToInt32(result ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Username}", request.Username);
            throw;
        }
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        // SECURE: Don't return password field
        const string query = "SELECT Id, Username, Email, Role, CreatedAt, IsActive FROM Users WHERE IsActive = 1";
        var users = new List<User>();
        
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            using var command = new SqlCommand(query, connection);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    Role = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4),
                    IsActive = reader.GetBoolean(5)
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            throw;
        }
        
        return users;
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        const string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username";
        
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("Username", username);
            
            await connection.OpenAsync();
            var count = (int)await command.ExecuteScalarAsync();
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking username existence");
            throw;
        }
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        const string query = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
        
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("Email", email);
            
            await connection.OpenAsync();
            var count = (int)await command.ExecuteScalarAsync();
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email existence");
            throw;
        }
    }
}