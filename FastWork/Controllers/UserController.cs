﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repo.Entities;
using Service.Helper;
using Service;
using Service.DTO;
using Service.Services.EmailService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Repo.Data;
using Service.Services.CategoryService;

namespace FastWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailSenderService;
        private readonly ICategoryService _categoryService;
        private readonly TheShineDbContext _context;
        private readonly string _frontendUrl;

        public UserController(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IConfiguration configuration,
            IEmailService emailSenderService,
            ICategoryService categoryService,
            TheShineDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailSenderService = emailSenderService;
            _categoryService = categoryService;
            _frontendUrl = configuration["Url:Frontend"] ?? "https://localhost:5044";
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost("admin")]
        public async Task<IActionResult> Register()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
                return BadRequest(new AppResponse<object>().SetErrorResponse("ModelState", errors));
            }

            var existingUser = await _userManager.FindByEmailAsync("admin");
            if (existingUser != null)
            {
                return BadRequest(new AppResponse<object>().SetErrorResponse("Email", new[] { "Email is already registered." }));
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "admin",
                Email = "admin@gmail.com",
                Name = "admin",
                Phone = "admin",
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, "Admin1?");
            if (result.Succeeded)
            {
                _categoryService.AddSample();
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Admin", NormalizedName = "ADMIN" });
                    if (!roleResult.Succeeded)
                    {
                        await _userManager.DeleteAsync(user);
                        return BadRequest(new AppResponse<object>().SetErrorResponse("RoleCreation", roleResult.Errors.Select(e => e.Description).ToArray()));
                    }
                }

                var roleAssignmentResult = await _userManager.AddToRoleAsync(user, "Admin");
                if (!roleAssignmentResult.Succeeded)
                {
                    Console.WriteLine($"Error adding role: {string.Join(", ", roleAssignmentResult.Errors.Select(e => e.Description))}");
                    await _userManager.DeleteAsync(user);
                    return BadRequest(new AppResponse<object>().SetErrorResponse("RoleAssignment", roleAssignmentResult.Errors.Select(e => e.Description).ToArray()));
                }

                Console.WriteLine($"Role 'Admin' assigned to user {user.Email}");

                return Ok(new AppResponse<object>().SetSuccessResponse(null, "Message", "Admin creation successfully."));
            }

            var identityErrors = result.Errors
                .Where(e => e.Code != "DuplicateUserName")
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());
            return BadRequest(new AppResponse<object>().SetErrorResponse(identityErrors));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
                return BadRequest(new AppResponse<object>().SetErrorResponse("ModelState", errors));
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest(new AppResponse<object>().SetErrorResponse("Email", new[] { "Email is already registered." }));
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = model.Email,
                Email = model.Email,
                Name = model.FullName,
                Phone = model.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("Candidate"))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Candidate", NormalizedName = "CANDIDATE" });
                    if (!roleResult.Succeeded)
                    {
                        await _userManager.DeleteAsync(user);
                        return BadRequest(new AppResponse<object>().SetErrorResponse("RoleCreation", roleResult.Errors.Select(e => e.Description).ToArray()));
                    }
                }

                var roleAssignmentResult = await _userManager.AddToRoleAsync(user, "Candidate");
                if (!roleAssignmentResult.Succeeded)
                {
                    Console.WriteLine($"Error adding role: {string.Join(", ", roleAssignmentResult.Errors.Select(e => e.Description))}");
                    await _userManager.DeleteAsync(user);
                    return BadRequest(new AppResponse<object>().SetErrorResponse("RoleAssignment", roleAssignmentResult.Errors.Select(e => e.Description).ToArray()));
                }

                Candidate candidate = new Candidate()
                {
                    CandidateId = Guid.NewGuid(),
                    Education = "Graduated",
                    Gender = "Male",
                    IncomeRange = "$2000",
                    Status = "Intern",
                    Verified = false,
                    Featured = false,
                    UserId = user.Id,
                    User = user
                };
                await _context.Candidates.AddAsync(candidate);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Role 'Candidate' assigned to user {user.Email}");

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = $"{_frontendUrl}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
                var emailBody = EmailBodyTemplate.GetRegistrationConfirmationEmail(user.Name, confirmationLink);

                _emailSenderService.SendEmail(user.Email, "Confirm Your Email", emailBody);

                return Ok(new AppResponse<object>().SetSuccessResponse(null, "Message", "Registration successful. Check your email to confirm your account."));
            }

            var identityErrors = result.Errors
                .Where(e => e.Code != "DuplicateUserName")
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());
            return BadRequest(new AppResponse<object>().SetErrorResponse(identityErrors));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var jwtConfig = _configuration.GetSection("Jwt").Get<Dictionary<string, string>>();
            Console.WriteLine("Jwt Config in Controller: " + (jwtConfig != null ? string.Join(", ", jwtConfig.Select(kvp => $"{kvp.Key}: {kvp.Value}")) : "null"));
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized(new AppResponse<object>().SetErrorResponse("Credentials", new[] { "Invalid credentials." }));
            }

            if (!user.EmailConfirmed)
            {
                return Unauthorized(new AppResponse<object>().SetErrorResponse("Email", new[] { "Email not confirmed." }));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            var accessToken = GenerateJwtToken(user, role);
            Console.WriteLine($"Generated Access Token: {accessToken}"); // Log the token immediately after generation
            var refreshToken = await GenerateRefreshToken(user);

            return Ok(new AppResponse<object>().SetSuccessResponse(new { AccessToken = accessToken, RefreshToken = refreshToken }, "Message", "Login successful."));
        }

        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new AppResponse<object>().SetErrorResponse("Email", new[] { "User not found." }));
            }

            if (user.EmailConfirmed)
            {
                return BadRequest(new AppResponse<object>().SetErrorResponse("Email", new[] { "Email already confirmed." }));
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"{_frontendUrl}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            var emailBody = EmailBodyTemplate.GetRegistrationConfirmationEmail(user.Name, confirmationLink);

            _emailSenderService.SendEmail(user.Email, "Confirm Your Email", emailBody);

            return Ok(new AppResponse<object>().SetSuccessResponse(null, "Message", "Confirmation email resent successfully."));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new AppResponse<object>().SetErrorResponse("Email", new[] { "User not found." }));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"{_frontendUrl}/reset-password?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";
            var emailBody = EmailBodyTemplate.GetPasswordResetEmail(user.Name, resetLink);

            _emailSenderService.SendEmail(user.Email, "Reset Your Password", emailBody);

            return Ok(new AppResponse<object>().SetSuccessResponse(null, "Message", "Password reset link sent to your email."));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new AppResponse<object>().SetErrorResponse("Email", new[] { "User not found." }));
            }

            var decodedToken = Uri.UnescapeDataString(model.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return BadRequest(new AppResponse<object>().SetErrorResponse("IdentityErrors", errors));
            }

            return Ok(new AppResponse<object>().SetSuccessResponse(null, "Message", "Password reset successful."));
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new AppResponse<object>().SetErrorResponse("User", new[] { "Invalid user." }));
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(new AppResponse<object>().SetErrorResponse("EmailConfirmation", new[] { "Email confirmation failed." }));
            }
            CandidateRegisterDto candidateRegisterDto = new CandidateRegisterDto()
            {
                Education = "haha",
                Gender = "male",
                IncomeRange = "1000-2000",
                Status = "Full-time"
            };
            var res = RegisterCandidate(candidateRegisterDto);
            return Ok(new AppResponse<object>().SetSuccessResponse(null, "Message", "Email confirmed successfully."));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return BadRequest(new AppResponse<object>().SetErrorResponse("User", new[] { "User not found." }));
            }

            var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
            if (storedToken != model.RefreshToken)
            {
                return Unauthorized(new AppResponse<object>().SetErrorResponse("Token", new[] { "Invalid refresh token." }));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            var newAccessToken = GenerateJwtToken(user, role);
            var newRefreshToken = await GenerateRefreshToken(user);

            return Ok(new AppResponse<object>().SetSuccessResponse(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken }, "Message", "Token refreshed successfully."));
        }

        [Authorize]
        [HttpGet("user-info")]
        public IActionResult GetUserInfo()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Unauthorized(new
                {
                    Message = "Not authenticated",
                    ReceivedToken = Request.Headers["Authorization"],
                    AuthenticationType = User.Identity?.AuthenticationType,
                    Exception = HttpContext.Features.Get<Exception>()?.Message
                });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = _userManager.FindByIdAsync(userId).Result;
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpGet("test-auth")]
        public IActionResult TestAuth()
        {
            Console.WriteLine($"Test Auth Processed at {DateTime.Now}: Path={HttpContext.Request.Path}, Method={HttpContext.Request.Method}");
            return Ok(new { Message = "Test endpoint working", Time = DateTime.Now });
        }

        [AllowAnonymous]
        [HttpGet("echo")]
        public IActionResult Echo()
        {
            Console.WriteLine($"Echo Processed at {DateTime.Now}: Path={HttpContext.Request.Path}, Method={HttpContext.Request.Method}, AuthHeader={Request.Headers["Authorization"]}");
            return Ok(new { Message = "Echo endpoint working", Token = Request.Headers["Authorization"], Time = DateTime.Now });
        }
        [AllowAnonymous]
        [HttpPost("validate-token")]
        public IActionResult ValidateToken([FromBody] string token)
        {
            Console.WriteLine($"Received Token in Body: {token}");
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = _configuration["Jwt:Key"];
            Console.WriteLine($"Validation Config - Issuer: {issuer}, Audience: {audience}, Key: {key}");
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
                Console.WriteLine($"Validation Parameters - ValidIssuer: {validationParameters.ValidIssuer}");
                tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                return Ok(new { Message = "Token is valid", Claims = jwtToken.Claims.Select(c => new { c.Type, c.Value }) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Token validation failed", Error = ex.Message });
            }
        }
        private string GenerateJwtToken(User user, string? role)
        {
            Console.WriteLine("Token Generation Key: " + _configuration["Jwt:Key"]);
            Console.WriteLine($"Issuer: {_configuration["Jwt:Issuer"]}, Audience: {_configuration["Jwt:Audience"]}");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, role ?? "Customer")
    };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpirationMinutes"])),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine($"Generated Token Payload: {string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}"))}");
            return tokenString;
        }

        private async Task<string> GenerateRefreshToken(User user)
        {
            var refreshToken = Guid.NewGuid().ToString();
            await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", refreshToken);
            return refreshToken;
        }
        [HttpPost("register-recruiter")]
        public async Task<IActionResult> RegisterRecruiter([FromBody] RecruiterRegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
                return BadRequest(new AppResponse<object>().SetErrorResponse("ModelState", errors));
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest(new AppResponse<object>().SetErrorResponse("Email", new[] { "Email is already registered." }));
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = model.Email,
                Email = model.Email,
                Name = model.FullName,
                Phone = model.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                try
                {
                    // Create Recruiter role if it doesn't exist
                    if (!await _roleManager.RoleExistsAsync("Recruiter"))
                    {
                        var role = new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Recruiter", NormalizedName = "RECRUITER" };
                        var roleResult = await _roleManager.CreateAsync(role);
                        if (!roleResult.Succeeded)
                        {
                            await _userManager.DeleteAsync(user);
                            return BadRequest(new AppResponse<object>().SetErrorResponse("RoleCreation", roleResult.Errors.Select(e => e.Description).ToArray()));
                        }
                    }

                    // Assign the role to the recruiter
                    var roleAssignmentResult = await _userManager.AddToRoleAsync(user, "Recruiter");
                    if (!roleAssignmentResult.Succeeded)
                    {
                        await _userManager.DeleteAsync(user);
                        return BadRequest(new AppResponse<object>().SetErrorResponse("RoleAssignment", roleAssignmentResult.Errors.Select(e => e.Description).ToArray()));
                    }

                    // Create Recruiter entity
                    var recruiter = new Recruiter
                    {
                        RecruiterId = Guid.NewGuid(),
                        UserId = user.Id,
                        CompanyType = model.CompanyType,
                        Scale = model.Scale,
                        ContactPhone = model.ContactPhone,
                        Verified = false
                    };

                    await _context.Recruiters.AddAsync(recruiter);
                    await _context.SaveChangesAsync();

                    // Send email confirmation
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = $"{_frontendUrl}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
                    var emailBody = EmailBodyTemplate.GetRegistrationConfirmationEmail(user.Name, confirmationLink);

                    _emailSenderService.SendEmail(user.Email, "Confirm Your Email", emailBody);

                    return Ok(new AppResponse<object>().SetSuccessResponse(null, "Message", "Recruiter registration successful. Check your email to confirm your account."));
                }
                catch (Exception ex)
                {
                    // Rollback user creation if any error occurs
                    await _userManager.DeleteAsync(user);
                    return StatusCode(StatusCodes.Status500InternalServerError, new AppResponse<object>().SetErrorResponse("ServerError", new[] { $"An error occurred: {ex.Message}" }));
                }
            }

            var identityErrors = result.Errors
                .Where(e => e.Code != "DuplicateUserName")
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());
            return BadRequest(new AppResponse<object>().SetErrorResponse(identityErrors));
        }

        [HttpPost("register-candidate")]
        public async Task<IActionResult> RegisterCandidate([FromBody] CandidateRegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
                return BadRequest(new AppResponse<object>().SetErrorResponse("ModelState", errors));
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new AppResponse<object>().SetErrorResponse("User", new[] { "User not authenticated." }));
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new AppResponse<object>().SetErrorResponse("User", new[] { "User not found." }));
            }
            Candidate candidate = new Candidate()
            {
                CandidateId = Guid.NewGuid(),
                Education = model.Education,
                Gender = model.Gender,
                IncomeRange = model.IncomeRange,
                Status = model.Status,
                Verified = false,
                Featured = false,
                UserId = user.Id,
                User = user
            };
            User existingCandidate = await _context.Users.Include(u => u.Candidate).FirstOrDefaultAsync(u => u.Id == user.Id);
            if (existingCandidate?.Candidate != null)
            {
                return BadRequest(new AppResponse<object>().SetErrorResponse("Candidate", new[] { "Candidate already registered." }));
            }
            await _context.Candidates.AddAsync(candidate);
            await _context.SaveChangesAsync();
            return Ok(new AppResponse<object>().SetSuccessResponse(candidate, "Message", "Candidate registration successful."));
        }

        public sealed record RegisterDto(string Email, string FullName, string PhoneNumber, string Password);
        public sealed record LoginDto(string Email, string Password);
        public sealed record ResendConfirmationDto(string Email);
        public sealed record ForgotPasswordDto(string Email);
        public sealed record ResetPasswordDto(string Email, string Token, string NewPassword);
        public sealed record RefreshTokenDto(string UserId, string RefreshToken);
        public sealed record RecruiterRegisterDto
        {
            [Required]
            [EmailAddress]
            public string Email { get; init; }
            [Required]
            public string FullName { get; init; }
            [Required]
            [Phone]
            public string PhoneNumber { get; init; }
            [Required]
            [MinLength(6)]
            public string Password { get; init; }
            [Required]
            [StringLength(50)]
            public string CompanyType { get; init; }
            [Required]
            [StringLength(50)]
            public string Scale { get; init; }
            [Required]
            [Phone]
            public string ContactPhone { get; init; }
        }
        public sealed record CandidateRegisterDto
        {
            [Required]
            [StringLength(50)]
            public string Status { get; init; }
            [Required]
            [StringLength(50)]
            public string Gender { get; init; }
            [Required]
            [StringLength(50)]
            public string Education { get; init; }
            [Required]
            [StringLength(50)]
            public string IncomeRange { get; init; }
        }
    }
}