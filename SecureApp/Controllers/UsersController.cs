using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SecureApp.Application.Services;
using Shared.Domain.Models;
using System.Security.Claims;

namespace SecureApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // SECURE: Require authentication for all endpoints
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ITokenService tokenService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous] // SECURE: Allow anonymous access for login
    // SECURE: Rate limiting handled by middleware
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // SECURE: Input validation is handled by FluentValidation middleware
        var user = await _userService.AuthenticateAsync(request);
        if (user != null)
        {
            var token = _tokenService.GenerateToken(user);
            
            // SECURE: Return only necessary data, no sensitive information
            return Ok(new 
            { 
                Message = "Login successful", 
                Token = token,
                User = new { user.Id, user.Username, user.Email, user.Role }
            });
        }
        
        // SECURE: Generic error message to prevent username enumeration
        return Unauthorized(new { Message = "Invalid credentials" });
    }

    [HttpPost("register")]
    [AllowAnonymous] // SECURE: Allow anonymous access for registration
    // SECURE: Rate limiting handled by middleware
    public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
    {
        try
        {
            // SECURE: Input validation handled by FluentValidation
            var userId = await _userService.CreateUserAsync(request);
            
            _logger.LogInformation("User registered successfully: {Username}", request.Username);
            return Ok(new { Message = "User created successfully", UserId = userId });
        }
        catch (InvalidOperationException ex)
        {
            // SECURE: Return specific validation errors
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")] // SECURE: Only admins can view all users
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        
        // SECURE: Log access to sensitive data
        _logger.LogInformation("Admin {Username} accessed all users list", User.Identity?.Name);
        
        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "UserOrAdmin")] // SECURE: Authenticated users only
    public async Task<IActionResult> GetUser(int id)
    {
        // SECURE: Authorization - users can only access their own data unless admin
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
        
        if (currentUserId != id && currentUserRole != "Admin")
        {
            _logger.LogWarning("Unauthorized access attempt by user {UserId} to user {TargetId}", currentUserId, id);
            return Forbid();
        }
        
        var user = await _userService.GetUserAsync(id);
        if (user != null)
        {
            return Ok(user);
        }
        return NotFound();
    }

    [HttpGet("profile")]
    [Authorize] // SECURE: Get current user's profile
    public async Task<IActionResult> GetProfile()
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var user = await _userService.GetUserAsync(currentUserId);
        
        if (user != null)
        {
            return Ok(user);
        }
        return NotFound();
    }

    [HttpGet("admin/sensitive-data")]
    [Authorize(Policy = "AdminOnly")] // SECURE: Admin-only endpoint with proper authorization
    public async Task<IActionResult> GetSensitiveData()
    {
        // SECURE: Log access to sensitive endpoints
        _logger.LogWarning("Sensitive data accessed by admin: {Username}", User.Identity?.Name);
        
        // SECURE: Return sanitized data, no actual secrets
        var sanitizedInfo = new
        {
            Message = "This endpoint contains sensitive administrative data",
            AccessedBy = User.Identity?.Name,
            AccessTime = DateTime.UtcNow,
            Note = "Actual sensitive data would be properly secured and audited"
        };
        
        return Ok(sanitizedInfo);
    }
}