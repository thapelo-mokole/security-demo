using InsecureApp.Data;
using InsecureApp.Services;

var builder = WebApplication.CreateBuilder(args);

// SECURITY RISK: No security headers, CORS, or authentication configured
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SECURITY RISK: Direct instantiation without DI, hardcoded connection strings
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<UserService>();

var app = builder.Build();

// SECURITY RISK: Swagger exposed in all environments
app.UseSwagger();
app.UseSwaggerUI();

// SECURITY RISK: No security middleware, HTTPS redirection, or rate limiting
app.MapControllers();

app.Run();