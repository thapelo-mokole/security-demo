# Security Demonstration: Insecure vs Secure C# Applications

This solution demonstrates the stark differences between insecure and secure coding practices in C# web applications, following Clean Architecture principles.

## 🏗️ Solution Structure

```
SecurityDemo/
├── InsecureApp/          # Demonstrates vulnerable practices
├── SecureApp/            # Demonstrates secure practices  
├── Shared.Domain/        # Common domain models
└── README.md            # This documentation
```

## 🚨 InsecureApp - What NOT to Do

### Security Vulnerabilities Demonstrated

#### 1. **SQL Injection**
- **Location**: `InsecureApp/Data/UserRepository.cs`
- **Issue**: Direct string concatenation in SQL queries
- **Risk**: Attackers can execute arbitrary SQL commands

```csharp
// VULNERABLE CODE
var query = $"SELECT * FROM Users WHERE Username = '{username}'";
```

#### 2. **Hardcoded Secrets**
- **Location**: `InsecureApp/Data/UserRepository.cs`
- **Issue**: Database credentials in source code
- **Risk**: Credentials exposed in version control

```csharp
// VULNERABLE CODE
private readonly string _connectionString = "Server=localhost;Database=TestDB;User Id=sa;Password=Password123!;";
```

#### 3. **Plaintext Password Storage**
- **Location**: `InsecureApp/Data/UserRepository.cs`
- **Issue**: Passwords stored and compared in plaintext
- **Risk**: Complete credential compromise if database is breached

#### 4. **Information Disclosure**
- **Location**: `InsecureApp/Controllers/UsersController.cs`
- **Issue**: Detailed error messages and stack traces exposed
- **Risk**: Reveals internal system architecture to attackers

#### 5. **No Authentication/Authorization**
- **Location**: All controllers
- **Issue**: No access controls on sensitive endpoints
- **Risk**: Unauthorized access to all system functions

#### 6. **No Input Validation**
- **Location**: All endpoints
- **Issue**: No validation of user input
- **Risk**: Various injection attacks and data corruption

## 🔒 SecureApp - Security Best Practices

### Security Controls Implemented

#### 1. **Parameterized Queries**
- **Location**: `SecureApp/Infrastructure/Repositories/UserRepository.cs`
- **Protection**: Prevents SQL injection attacks

```csharp
// SECURE CODE
const string query = "SELECT * FROM Users WHERE Username = @Username";
command.Parameters.AddWithValue("@Username", username);
```

#### 2. **Secrets Management**
- **Location**: `SecureApp/Program.cs`, `appsettings.json`
- **Protection**: Configuration-based connection strings, User Secrets for development

#### 3. **Password Hashing**
- **Location**: `SecureApp/Application/Services/UserService.cs`
- **Protection**: BCrypt hashing with salt

```csharp
// SECURE CODE
var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password, BCrypt.Net.BCrypt.GenerateSalt(12));
```

#### 4. **JWT Authentication**
- **Location**: `SecureApp/Program.cs`, `SecureApp/Application/Services/TokenService.cs`
- **Protection**: Stateless, secure token-based authentication

#### 5. **Role-Based Authorization (RBAC)**
- **Location**: `SecureApp/Controllers/UsersController.cs`
- **Protection**: Granular access control based on user roles

```csharp
// SECURE CODE
[Authorize(Policy = "AdminOnly")]
public async Task<IActionResult> GetAllUsers()
```

#### 6. **Input Validation**
- **Location**: `SecureApp/Validators/`
- **Protection**: FluentValidation for comprehensive input sanitization

#### 7. **Rate Limiting**
- **Location**: `SecureApp/Program.cs`
- **Protection**: Prevents brute force and DoS attacks

#### 8. **Security Headers**
- **Location**: `SecureApp/Middleware/SecurityHeadersMiddleware.cs`
- **Protection**: Defense against XSS, clickjacking, and other client-side attacks

#### 9. **Centralized Exception Handling**
- **Location**: `SecureApp/Middleware/ExceptionHandlingMiddleware.cs`
- **Protection**: Prevents information leakage through error messages

#### 10. **Comprehensive Logging**
- **Location**: Throughout SecureApp using Serilog
- **Protection**: Security event monitoring and audit trails

## 🏛️ Clean Architecture Implementation

### Domain Layer (`Shared.Domain`)
- **Purpose**: Core business entities and contracts
- **Users**: Both applications for consistent data models
- **Security**: Input/output DTOs separate from entities

### Application Layer (`SecureApp/Application`)
- **Purpose**: Business logic and use cases
- **Users**: Service layer consumers
- **Security**: Input validation, business rule enforcement

### Infrastructure Layer (`SecureApp/Infrastructure`)
- **Purpose**: Data access and external service integration
- **Users**: Application services
- **Security**: Parameterized queries, connection management

### Presentation Layer (`Controllers`)
- **Purpose**: HTTP API endpoints
- **Users**: Client applications
- **Security**: Authentication, authorization, rate limiting

## 🛡️ Security Principles Applied

### 1. **Defense in Depth**
Multiple security layers:
- Network (HTTPS, security headers)
- Application (authentication, authorization)
- Data (encryption, parameterized queries)

### 2. **Principle of Least Privilege**
- Users can only access their own data
- Admin functions require admin role
- JWT tokens have limited lifetime

### 3. **Fail Securely**
- Generic error messages
- Secure defaults in configuration
- Graceful degradation

### 4. **Security by Design**
- Security considerations in architecture
- Secure coding patterns throughout
- Regular security reviews built into process

## 📊 Monitoring and Governance

### Continuous Monitoring
- **Health Checks**: `/health` endpoint for system monitoring
- **Structured Logging**: Serilog with file and console outputs
- **Security Events**: Authentication failures, authorization violations
- **Performance Metrics**: Request timing, error rates

### Audit Logging
```csharp
// Example security event logging
_logger.LogWarning("Unauthorized access attempt by user {UserId} to user {TargetId}", currentUserId, id);
```

### Compliance Features
- **Data Protection**: No sensitive data in logs
- **Access Control**: RBAC implementation
- **Audit Trail**: All security events logged
- **Data Retention**: Configurable log retention policies

## 🧪 Testing Strategy

### Security Testing Approaches

#### 1. **Static Analysis (SAST)**
- Code review for security vulnerabilities
- Automated scanning for common issues
- Dependency vulnerability scanning

#### 2. **Dynamic Testing (DAST)**
- Penetration testing of running application
- Authentication bypass attempts
- Input validation testing

#### 3. **Interactive Testing (IAST)**
- Runtime security monitoring
- Real-time vulnerability detection
- Performance impact analysis

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB acceptable for demo)
- Visual Studio 2022 or VS Code

### Setup Instructions

1. **Clone and Build**
```bash
git clone <repository>
cd SecurityDemo
dotnet build
```

2. **Configure Secrets (SecureApp)**
```bash
cd SecureApp
dotnet user-secrets set "Jwt:Key" "your-super-secret-jwt-key-here-minimum-32-characters"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
```

3. **Database Setup**
```sql
-- Create tables for both applications
CREATE TABLE Users (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Username nvarchar(50) NOT NULL,
    Email nvarchar(100) NOT NULL,
    Password nvarchar(255) NOT NULL,
    Role nvarchar(20) NOT NULL DEFAULT 'User',
    CreatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    IsActive bit NOT NULL DEFAULT 1
);
```

4. **Run Applications**
```bash
# Terminal 1 - Insecure App
cd InsecureApp
dotnet run

# Terminal 2 - Secure App  
cd SecureApp
dotnet run
```

## 🎯 Key Takeaways

### For Developers
- **Never trust user input** - Always validate and sanitize
- **Use parameterized queries** - Prevent SQL injection
- **Hash passwords** - Never store plaintext credentials
- **Implement proper authentication** - Use industry standards like JWT
- **Apply authorization** - Control access to resources
- **Log security events** - Enable monitoring and incident response

### For Security Teams
- **Code reviews are critical** - Catch vulnerabilities early
- **Automated scanning helps** - But manual review is essential
- **Defense in depth works** - Multiple security layers provide better protection
- **Monitoring is key** - Detect and respond to security incidents quickly

### For Management
- **Security is not optional** - It's a business requirement
- **Training matters** - Invest in developer security education
- **Tools help** - But process and culture are more important
- **Compliance follows security** - Good security practices enable compliance

## 📚 Additional Resources

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [Microsoft Security Development Lifecycle](https://www.microsoft.com/en-us/securityengineering/sdl)
- [NIST Cybersecurity Framework](https://www.nist.gov/cyberframework)
- [Clean Architecture by Robert Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

**⚠️ Important**: The InsecureApp is for educational purposes only. Never deploy code with these vulnerabilities to production environments.