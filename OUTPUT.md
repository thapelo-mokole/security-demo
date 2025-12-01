📁 Solution Structure Created
Core Projects:
InsecureApp - Demonstrates vulnerable practices (SQL injection, hardcoded secrets, no authentication)

SecureApp - Implements security best practices (JWT auth, parameterized queries, input validation)

Shared.Domain - Common domain models

Key Security Demonstrations:
🚨 InsecureApp Vulnerabilities:
SQL injection through string concatenation

Hardcoded database credentials in source code

Plaintext password storage and comparison

No authentication or authorization

Information disclosure through detailed error messages

No input validation or rate limiting

🔒 SecureApp Protections:
Parameterized queries preventing SQL injection

Configuration-based secrets management

BCrypt password hashing with salt

JWT authentication with role-based authorization (RBAC)

Comprehensive input validation using FluentValidation

Rate limiting and security headers middleware

Centralized exception handling

Structured logging with Serilog

Health checks for monitoring

Clean Architecture Implementation:
Domain Layer: Shared entities and contracts

Application Layer: Business logic with security validation

Infrastructure Layer: Secure data access with parameterized queries

Presentation Layer: Controllers with authentication/authorization

Security Principles Applied:
Defense in Depth: Multiple security layers

Principle of Least Privilege: Role-based access control

Fail Securely: Generic error messages, secure defaults

Security by Design: Built-in security from architecture level

Documentation Provided:
README.md: Comprehensive overview and security explanations

SECURITY_COMPARISON.md: Side-by-side vulnerability analysis

SETUP.md: Complete setup and testing guide

setup-database.sql: Database initialization script

Testing & Monitoring:
Sample attack scenarios for SQL injection testing

Rate limiting demonstrations

Security event logging and audit trails

Health check endpoints

Structured logging for continuous monitoring

The solution demonstrates real-world security vulnerabilities and their proper mitigations, following Clean Architecture principles while showing how secure coding practices, data protection, governance, and monitoring integrate into a C# application architecture