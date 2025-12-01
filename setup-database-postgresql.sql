-- Database Setup Script for Security Demo (PostgreSQL)
-- Run this script to create the necessary database tables

-- Create database (if needed)
-- CREATE DATABASE securitydemo;
-- \c securitydemo;

-- Create Users table for both applications
CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    role VARCHAR(20) NOT NULL DEFAULT 'User',
    createdat TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    isactive BOOLEAN NOT NULL DEFAULT TRUE
);

-- Create index for performance
CREATE INDEX IF NOT EXISTS ix_users_username ON users(username);
CREATE INDEX IF NOT EXISTS ix_users_email ON users(email);

-- Insert sample data for testing
-- Note: InsecureApp stores plaintext passwords, SecureApp uses BCrypt hashes

-- Sample user for InsecureApp (plaintext password: "password123")
INSERT INTO users (username, email, password, role) 
VALUES ('testuser', 'test@example.com', 'password123', 'User')
ON CONFLICT (username) DO NOTHING;

-- Sample admin for InsecureApp (plaintext password: "admin123")
INSERT INTO users (username, email, password, role) 
VALUES ('admin', 'admin@example.com', 'admin123', 'Admin')
ON CONFLICT (username) DO NOTHING;

-- Sample user for SecureApp (BCrypt hash of "password123")
INSERT INTO users (username, email, password, role) 
VALUES ('secureuser', 'secure@example.com', '$2a$12$qXQ53JYallQjUzNhgAyf9uemM.j2GfzCQmyS6t82I2VokRIlHaBQu', 'User')
ON CONFLICT (username) DO NOTHING;

-- Sample admin for SecureApp (BCrypt hash of "admin123")
INSERT INTO users (username, email, password, role) 
VALUES ('secureadmin', 'secureadmin@example.com', '$2a$12$YILozhn0Ry9wGboKD0SnKeoqSj4OE2EuGyvs7wanrHs4yhq4i5sIK', 'Admin')
ON CONFLICT (username) DO NOTHING;

-- Verify data
SELECT id, username, email, role, createdat, isactive FROM users;

-- PostgreSQL uses RAISE NOTICE instead of PRINT
DO $$
BEGIN
    RAISE NOTICE 'Database setup completed successfully!';
    RAISE NOTICE 'Test credentials:';
    RAISE NOTICE 'InsecureApp - User: testuser/password123, Admin: admin/admin123';
    RAISE NOTICE 'SecureApp - User: secureuser/password123, Admin: secureadmin/admin123';
END $$;