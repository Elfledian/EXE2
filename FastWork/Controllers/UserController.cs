using Microsoft.AspNetCore.Identity;
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
        private readonly string _frontendUrl;

        public UserController(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IConfiguration configuration,
            IEmailService emailSenderService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailSenderService = emailSenderService;
            _frontendUrl = configuration["Url:Frontend"] ?? "https://youtube.com";
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
                Id = Guid.NewGuid(), // Set the Id property directly
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
                // Ensure the "Customer" role exists
                if (!await _roleManager.RoleExistsAsync("Candidate"))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Candidate", NormalizedName = "CANDIDATE" });
                    if (!roleResult.Succeeded)
                    {
                        // Rollback user creation if role creation fails
                        await _userManager.DeleteAsync(user);
                        return BadRequest(new AppResponse<object>().SetErrorResponse("RoleCreation", roleResult.Errors.Select(e => e.Description).ToArray()));
                    }
                }

                // Add the user to the "Customer" role
                var roleAssignmentResult = await _userManager.AddToRoleAsync(user, "Candidate");
                if (!roleAssignmentResult.Succeeded)
                {
                    // Log the error and return a failure response
                    Console.WriteLine($"Error adding role: {string.Join(", ", roleAssignmentResult.Errors.Select(e => e.Description))}");
                    // Rollback user creation
                    await _userManager.DeleteAsync(user);
                    return BadRequest(new AppResponse<object>().SetErrorResponse("RoleAssignment", roleAssignmentResult.Errors.Select(e => e.Description).ToArray()));
                }

                Console.WriteLine($"Role 'Customer' assigned to user {user.Email}");

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

            return Ok(new AppResponse<object>().SetSuccessResponse(null, "Message", "Email confirmed successfully."));
        }

        private string GenerateJwtToken(User user, string? role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role ?? "Customer")
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateRefreshToken(User user)
        {
            var refreshToken = Guid.NewGuid().ToString();
            await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", refreshToken);
            return refreshToken;
        }

        public sealed record RegisterDto(string Email, string FullName, string PhoneNumber, string Password);
        public sealed record LoginDto(string Email, string Password);
        public sealed record ResendConfirmationDto(string Email);
        public sealed record ForgotPasswordDto(string Email);
        public sealed record ResetPasswordDto(string Email, string Token, string NewPassword);
    }
}
