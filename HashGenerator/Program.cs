using BCrypt.Net;

Console.WriteLine("BCrypt Hash Generator for Security Demo");
Console.WriteLine("========================================\n");

var password123 = BCrypt.Net.BCrypt.HashPassword("password123", 12);
var admin123 = BCrypt.Net.BCrypt.HashPassword("admin123", 12);

Console.WriteLine("Generated Hashes:");
Console.WriteLine($"password123: {password123}");
Console.WriteLine($"admin123: {admin123}");

Console.WriteLine("\nVerification:");
Console.WriteLine($"password123 verified: {BCrypt.Net.BCrypt.Verify("password123", password123)}");
Console.WriteLine($"admin123 verified: {BCrypt.Net.BCrypt.Verify("admin123", admin123)}");

Console.WriteLine("\n\nSQL INSERT Statements:");
Console.WriteLine("======================");
Console.WriteLine($"INSERT INTO users (username, email, password, role) VALUES ('secureuser', 'secure@example.com', '{password123}', 'User');");
Console.WriteLine($"INSERT INTO users (username, email, password, role) VALUES ('secureadmin', 'secureadmin@example.com', '{admin123}', 'Admin');");