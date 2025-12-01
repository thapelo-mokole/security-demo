# Security Demo UI

Interactive web interface to demonstrate security vulnerabilities and protections in real-time.

## 🚀 Quick Start

1. **Start Both Applications**
   ```bash
   # Terminal 1 - InsecureApp
   cd InsecureApp
   dotnet run
   
   # Terminal 2 - SecureApp
   cd SecureApp
   dotnet run
   ```

2. **Open the UI**
   - Simply open `index.html` in your web browser
   - Or use a local server:
     ```bash
     cd SecurityDemoUI
     python -m http.server 8080
     # Then open http://localhost:8080
     ```

## 🎯 Features

### Side-by-Side Comparison
- **Left Panel**: InsecureApp vulnerabilities
- **Right Panel**: SecureApp protections

### Interactive Tests

#### 1. SQL Injection
- **Insecure**: Try `admin' OR '1'='1` to bypass authentication
- **Secure**: Same input is safely handled with parameterized queries

#### 2. Authentication
- **Insecure**: Access all endpoints without any credentials
- **Secure**: JWT token required for protected endpoints

#### 3. Password Storage
- **Insecure**: Passwords stored in plaintext
- **Secure**: BCrypt hashing with salt + input validation

#### 4. Authorization
- **Insecure**: No access controls on sensitive data
- **Secure**: Role-based access control (RBAC)

## 📝 Test Credentials

### InsecureApp
- Username: `testuser` / Password: `password123`
- Admin: `admin` / Password: `admin123`

### SecureApp
- Username: `secureuser` / Password: `password123`
- Admin: `secureadmin` / Password: `admin123`

## 🔍 What You'll See

### Vulnerability Demonstrations
- ⚠️ SQL injection bypassing authentication
- ⚠️ Accessing all users without authentication
- ⚠️ Plaintext passwords in responses
- ⚠️ Sensitive data exposed without authorization

### Protection Demonstrations
- ✅ SQL injection attempts blocked
- ✅ 401 Unauthorized without JWT token
- ✅ Passwords hashed with BCrypt
- ✅ Input validation rejecting weak passwords
- ✅ Role-based access control enforced

## 🛠️ Troubleshooting

### CORS Issues
If you see CORS errors, add this to both apps' `Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// After var app = builder.Build();
app.UseCors();
```

### HTTPS Certificate
If you see certificate errors:
```bash
dotnet dev-certs https --trust
```

### Ports Not Available
- InsecureApp: Change port in `Properties/launchSettings.json`
- SecureApp: Change port in `Properties/launchSettings.json`
- Update `INSECURE_API` and `SECURE_API` constants in `index.html`

## 📚 Learning Points

Each test includes:
- **Vulnerability Badge**: Shows what's wrong
- **Protection Badge**: Shows what's right
- **Info Box**: Explains the security issue/protection
- **Code Snippets**: Shows actual code differences
- **Live Results**: Real-time API responses

## 🎓 Educational Use

This UI is perfect for:
- Security training sessions
- Code review demonstrations
- Developer education
- Security awareness programs
- Penetration testing practice

---

**⚠️ Warning**: Only use InsecureApp for educational purposes. Never deploy vulnerable code to production!