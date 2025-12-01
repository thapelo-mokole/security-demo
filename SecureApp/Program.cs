using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SecureApp.Application.Services;
using SecureApp.Infrastructure.Repositories;
using SecureApp.Middleware;
using SecureApp.Validators;
using Serilog;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

// SECURE: Structured logging with Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/secureapp-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// SECURE: JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Admin"));
});

// SECURE: Rate limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = 429;
    options.RealIpHeader = "X-Real-IP";
    options.GeneralRules = new List<AspNetCoreRateLimit.RateLimitRule>
    {
        new AspNetCoreRateLimit.RateLimitRule
        {
            Endpoint = "*/api/users/login",
            Period = "1m",
            Limit = 5
        }
    };
});
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

// SECURE: Input validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

// SECURE: Dependency injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// SECURE: Health checks
builder.Services.AddHealthChecks()
    .AddCheck("database", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// SECURE: Swagger only in development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

// SECURE: Security headers middleware
app.UseMiddleware<SecurityHeadersMiddleware>();

// SECURE: Exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// SECURE: HTTPS redirection
app.UseHttpsRedirection();

// SECURE: Rate limiting
app.UseIpRateLimiting();

// SECURE: Authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// SECURE: Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// SECURE: Health checks endpoint
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();