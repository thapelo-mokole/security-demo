using InsecureApp.Data;
using InsecureApp.Services;

var builder = WebApplication.CreateBuilder(args);

// SECURITY RISK: No security headers, CORS, or authentication configured
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "InsecureApp API", 
        Version = "v1",
        Description = "⚠️ INSECURE - Demonstrates security vulnerabilities for educational purposes"
    });
});

// SECURITY RISK: Direct instantiation without DI, hardcoded connection strings
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<UserService>();

var app = builder.Build();

// SECURITY RISK: Swagger exposed in all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "InsecureApp API v1");
    c.RoutePrefix = string.Empty; // Launch at root
});

// SECURITY RISK: No security middleware, HTTPS redirection, or rate limiting
app.UseCors();
app.MapControllers();

app.Run();