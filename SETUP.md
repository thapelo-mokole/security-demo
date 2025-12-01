# Setup Guide for Security Demo

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB, Express, or full version)
- Visual Studio 2022 or VS Code with C# extension

### Step 1: Clone and Build
```bash
git clone <your-repository-url>
cd Univen
dotnet build
```

### Step 2: Database Setup
1. **Create Database**
   ```sql
   CREATE DATABASE SecurityDemo;
   ```

2. **Run Setup Script**
   ```bash
   sqlcmd -S localhost -d SecurityDemo -i setup-database.sql
   ```
   
   Or execute `setup-database.sql` in SQL Server Management Studio

### Step 3: Configure SecureApp Secrets
```bash
cd SecureApp
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "your-super-secret-jwt-key-here-at-least-32-characters-long"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=SecurityDemo;Integrated Security=true;TrustServerCertificate=true;"
```

### Step 4: Run Applications

**Terminal 1 - InsecureApp:**
```bash
cd InsecureApp
dotnet run
# Runs on https://localhost:5001
```

**Terminal 2 - SecureApp:**
```bash
cd SecureApp  
dotnet run
# Runs on https://localhost:5002
```

## 🧪 Testing the Applications

### Test Credentials
- **InsecureApp**: `testuser/password123` or `admin/admin123`
- **SecureApp**: `secureuser/password123` or `secureadmin/admin123`

### API Endpoints

#### InsecureApp (Port 5001)
```bash
# Login (no security)
curl -X POST "https://localhost:5001/api/users/login" \
  -H "Content-Type: application/json" \
  -d '{"username": "testuser", "password": "password123"}'

# Get all users (no auth required)
curl -X GET "https://localhost:5001/api/users"

# Access sensitive data (no auth required)
curl -X GET "https://localhost:5001/api/users/admin/sensitive-data"
```

#### SecureApp (Port 5002)
```bash
# Login (returns JWT token)
curl -X POST "https://localhost:5002/api/users/login" \
  -H "Content-Type: application/json" \
  -d '{"username": "secureuser", "password": "password123"}'

# Get all users (requires admin token)
curl -X GET "https://localhost:5002/api/users" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"

# Get user profile (requires valid token)
curl -X GET "https://localhost:5002/api/users/profile" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

## 🔍 Security Testing Scenarios

### 1. SQL Injection Test (InsecureApp)
```bash
# This will demonstrate SQL injection vulnerability
curl -X POST "https://localhost:5001/api/users/login" \
  -H "Content-Type: application/json" \
  -d '{"username": "admin'\'''; SELECT * FROM Users; --", "password": "anything"}'
```

### 2. Authentication Bypass (InsecureApp)
```bash
# Access sensitive data without authentication
curl -X GET "https://localhost:5001/api/users/admin/sensitive-data"
# Should return sensitive information without any authentication
```

### 3. Rate Limiting Test (SecureApp)
```bash
# Test rate limiting on login endpoint
for i in {1..10}; do
  curl -X POST "https://localhost:5002/api/users/login" \
    -H "Content-Type: application/json" \
    -d '{"username": "wrong", "password": "wrong"}'
  echo "Request $i completed"
done
# Should see rate limiting after 5 requests
```

### 4. Input Validation Test (SecureApp)
```bash
# Test input validation
curl -X POST "https://localhost:5002/api/users/register" \
  -H "Content-Type: application/json" \
  -d '{"username": "a", "email": "invalid-email", "password": "weak"}'
# Should return validation errors
```

## 📊 Monitoring and Logs

### SecureApp Logs
- **Location**: `SecureApp/logs/`
- **Format**: Structured JSON logs with Serilog
- **Content**: Security events, authentication attempts, errors

### Health Checks
- **SecureApp**: `https://localhost:5002/health`
- **Response**: System health status

### Swagger Documentation
- **InsecureApp**: `https://localhost:5001/swagger` (always available)
- **SecureApp**: `https://localhost:5002/swagger` (development only)

## 🛠️ Development Environment

### Visual Studio Setup
1. Open `SecurityDemo.sln`
2. Set multiple startup projects:
   - Right-click solution → Properties
   - Select "Multiple startup projects"
   - Set both InsecureApp and SecureApp to "Start"

### VS Code Setup
1. Open workspace folder
2. Install recommended extensions:
   - C# Dev Kit
   - REST Client (for testing APIs)

### Database Connection Troubleshooting

**LocalDB Connection String:**
```json
"Server=(localdb)\\mssqllocaldb;Database=SecurityDemo;Trusted_Connection=true;MultipleActiveResultSets=true"
```

**SQL Server Express:**
```json
"Server=localhost\\SQLEXPRESS;Database=SecurityDemo;Trusted_Connection=true;TrustServerCertificate=true"
```

**Docker SQL Server:**
```bash
# Run SQL Server in Docker
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 --name sql-server \
  -d mcr.microsoft.com/mssql/server:2022-latest

# Connection string for Docker
"Server=localhost,1433;Database=SecurityDemo;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true"
```

## 🔧 Troubleshooting

### Common Issues

1. **Port Already in Use**
   ```bash
   # Change ports in launchSettings.json or use different ports
   dotnet run --urls "https://localhost:6001"
   ```

2. **Database Connection Failed**
   - Verify SQL Server is running
   - Check connection string in user secrets
   - Ensure database exists

3. **JWT Token Issues**
   - Verify JWT key is set in user secrets
   - Check token expiration (1 hour default)
   - Ensure proper Authorization header format

4. **HTTPS Certificate Issues**
   ```bash
   # Trust development certificate
   dotnet dev-certs https --trust
   ```

### Debugging Tips

1. **Enable Detailed Logging**
   ```json
   // In appsettings.Development.json
   "Logging": {
     "LogLevel": {
       "Default": "Debug",
       "Microsoft.AspNetCore": "Information"
     }
   }
   ```

2. **Check User Secrets**
   ```bash
   cd SecureApp
   dotnet user-secrets list
   ```

3. **Verify Database Data**
   ```sql
   SELECT * FROM Users;
   ```

## 📚 Next Steps

1. **Explore the Code**: Compare implementations between InsecureApp and SecureApp
2. **Run Security Tests**: Use the provided test scenarios
3. **Review Documentation**: Read README.md and SECURITY_COMPARISON.md
4. **Customize**: Modify the applications to test additional security scenarios
5. **Learn More**: Check the additional resources in the documentation

---

**⚠️ Important Security Note**: 
- InsecureApp is for educational purposes only
- Never deploy insecure code to production
- Always follow secure coding practices in real applications