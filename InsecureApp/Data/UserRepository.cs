using Npgsql;
using Shared.Domain.Models;

namespace InsecureApp.Data;

public class UserRepository
{
    // SECURITY RISK: Hardcoded connection string with credentials exposed
    private readonly string _connectionString = "Host=localhost;Database=securitydemo;Username=postgres;Password=P@ssw0rd;";

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        // SECURITY RISK: SQL Injection vulnerability - direct string concatenation
        var query = $"SELECT * FROM users WHERE username = '{username}'";
        
        using var connection = new NpgsqlConnection(_connectionString);
        using var command = new NpgsqlCommand(query, connection);
        
        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Email = reader.GetString(2),
                Password = reader.GetString(3), // SECURITY RISK: Returning plaintext password
                Role = reader.GetString(4),
                CreatedAt = reader.GetDateTime(5),
                IsActive = reader.GetBoolean(6)
            };
        }
        return null;
    }

    public async Task<int> CreateUserAsync(CreateUserRequest request)
    {
        // SECURITY RISK: SQL Injection + storing plaintext password
        var query = $@"INSERT INTO users (username, email, password, role, createdat, isactive) 
                      VALUES ('{request.Username}', '{request.Email}', '{request.Password}', '{request.Role}', CURRENT_TIMESTAMP, TRUE)
                      RETURNING id;";
        
        using var connection = new NpgsqlConnection(_connectionString);
        using var command = new NpgsqlCommand(query, connection);
        
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result ?? 0);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        // SECURITY RISK: No access control, returns sensitive data
        var query = "SELECT * FROM users";
        var users = new List<User>();
        
        using var connection = new NpgsqlConnection(_connectionString);
        using var command = new NpgsqlCommand(query, connection);
        
        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Email = reader.GetString(2),
                Password = reader.GetString(3), // SECURITY RISK: Exposing passwords
                Role = reader.GetString(4),
                CreatedAt = reader.GetDateTime(5),
                IsActive = reader.GetBoolean(6)
            });
        }
        return users;
    }
}