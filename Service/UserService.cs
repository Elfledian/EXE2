using Repo.Entities;
using Repo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Service
{
    public  class UserService
    {
        private readonly UserRepo _userRepo;

        public UserService() => _userRepo = new UserRepo();
        public UserService(UserRepo userRepo) => _userRepo = userRepo;

        public async Task<(User? User, string? Error)> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return (null, "Email and password are required");
            }

            var user = await _userRepo.GetUserByEmailAsync(email);
            if (user == null)
            {
                return (null, "Invalid email or password");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return (null, "Invalid email or password");
            }

            return (user, null);
        }

        public async Task<(User? User, string? Error)> SignupAsync(string email, string username,string name, string password, string role = "User")
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return (null, "All fields are required");
            }

            if (!IsValidEmail(email))
            {
                return (null, "Invalid email format");
            }

            if (!IsValidPassword(password))
            {
                return (null, "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character");
            }

            // Check if email already exists
            var existingUser = await _userRepo.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                return (null, "Email already registered");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Name = name,
                UserName = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepo.AddUserAsync(user);
            return (user, null);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPassword(string password)
        {
            try
            {
                var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
                return regex.IsMatch(password);
            }
            catch
            {
                return false;
            }
        }

    }
}
