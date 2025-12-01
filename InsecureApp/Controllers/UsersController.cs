using Microsoft.AspNetCore.Mvc;
using InsecureApp.Services;
using Shared.Domain.Models;

namespace InsecureApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService = new(); // SECURITY RISK: Direct instantiation

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            // SECURITY RISK: No rate limiting, input validation, or secure session management
            var user = await _userService.AuthenticateAsync(request);
            if (user != null)
            {
                // SECURITY RISK: Returning sensitive user data including password
                return Ok(new { Message = "Login successful", User = user });
            }
            
            // SECURITY RISK: Information disclosure - reveals if username exists
            return Unauthorized(new { Message = "Invalid username or password" });
        }
        catch (Exception ex)
        {
            // SECURITY RISK: Exposing internal error details
            return StatusCode(500, new { Message = ex.Message, StackTrace = ex.StackTrace });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
    {
        try
        {
            // SECURITY RISK: No input validation, CAPTCHA, or duplicate checking
            var userId = await _userService.CreateUserAsync(request);
            return Ok(new { Message = "User created successfully", UserId = userId });
        }
        catch (Exception ex)
        {
            // SECURITY RISK: Exposing database errors
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            // SECURITY RISK: No authentication or authorization required
            var users = await _userService.GetAllUsersAsync();
            return Ok(users); // SECURITY RISK: Returning all user data including passwords
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetUser(string username)
    {
        try
        {
            // SECURITY RISK: No authorization - any user can access other users' data
            var user = await _userService.GetUserAsync(username);
            if (user != null)
            {
                return Ok(user); // SECURITY RISK: Returning sensitive data
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("admin/sensitive-data")]
    public async Task<IActionResult> GetSensitiveData()
    {
        // SECURITY RISK: No authentication or role-based authorization
        var sensitiveInfo = new
        {
            DatabaseConnectionString = "Server=localhost;Database=TestDB;User Id=sa;Password=Password123!;",
            ApiKeys = new[] { "sk-1234567890abcdef", "pk-abcdef1234567890" },
            InternalUrls = new[] { "http://internal-api.company.com", "http://admin-panel.company.com" }
        };
        
        return Ok(sensitiveInfo);
    }
}