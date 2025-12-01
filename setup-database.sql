-- Database Setup Script for Security Demo
-- Run this script to create the necessary database tables

-- Create database (if needed)
-- CREATE DATABASE SecurityDemo;
-- USE SecurityDemo;

-- Create Users table for both applications
CREATE TABLE Users (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Username nvarchar(50) NOT NULL UNIQUE,
    Email nvarchar(100) NOT NULL UNIQUE,
    Password nvarchar(255) NOT NULL,
    Role nvarchar(20) NOT NULL DEFAULT 'User',
    CreatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    IsActive bit NOT NULL DEFAULT 1
);

-- Create index for performance
CREATE INDEX IX_Users_Username ON Users(Username);
CREATE INDEX IX_Users_Email ON Users(Email);

-- Insert sample data for testing
-- Note: InsecureApp stores plaintext passwords, SecureApp uses BCrypt hashes

-- Sample user for InsecureApp (plaintext password: "password123")
INSERT INTO Users (Username, Email, Password, Role) 
VALUES ('testuser', 'test@example.com', 'password123', 'User');

-- Sample admin for InsecureApp (plaintext password: "admin123")
INSERT INTO Users (Username, Email, Password, Role) 
VALUES ('admin', 'admin@example.com', 'admin123', 'Admin');

-- Sample user for SecureApp (BCrypt hash of "password123")
-- Hash generated with: BCrypt.Net.BCrypt.HashPassword("password123", BCrypt.Net.BCrypt.GenerateSalt(12))
INSERT INTO Users (Username, Email, Password, Role) 
VALUES ('secureuser', 'secure@example.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj/RK.s5uO9G', 'User');

-- Sample admin for SecureApp (BCrypt hash of "admin123")
INSERT INTO Users (Username, Email, Password, Role) 
VALUES ('secureadmin', 'secureadmin@example.com', '$2a$12$8k1lnD5DHbS8CdVfHIl9.eaLQvQI5JKlOhd5TtxMQJqhN8/LewdBPj', 'Admin');

-- Verify data
SELECT Id, Username, Email, Role, CreatedAt, IsActive FROM Users;

PRINT 'Database setup completed successfully!';
PRINT 'Test credentials:';
PRINT 'InsecureApp - User: testuser/password123, Admin: admin/admin123';
PRINT 'SecureApp - User: secureuser/password123, Admin: secureadmin/admin123';