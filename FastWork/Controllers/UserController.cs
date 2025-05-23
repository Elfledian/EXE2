using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repo.Entities;
using Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FastWork.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _config;
        private readonly UserService _userService;

        public UserController(IConfiguration config, UserService userService)
        {
            _config = config;
            _userService = userService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (user, error) = await _userService.LoginAsync(request.UserName, request.Password);

            if (user == null)
            {
                return Unauthorized(new { message = error ?? "Invalid credentials" });
            }

            var token = GenerateJSONWebToken(user);
            return Ok(new { token, userId = user.Id, userName = user.UserName, email = user.Email });
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            var (user, error) = await _userService.SignupAsync(request.Email, request.UserName,request.Name, request.Password);

            if (user == null)
            {
                return BadRequest(new { message = error ?? "Registration failed" });
            }

            var token = GenerateJSONWebToken(user);
            return Ok(new { token, userId = user.Id, userName = user.UserName, email = user.Email });
        }

        private string GenerateJSONWebToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                new Claim[]
                {
                    new(ClaimTypes.Name, user.UserName),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Role, user.Role),
                    new(ClaimTypes.NameIdentifier, user.Id.ToString())
                },
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public sealed record LoginRequest(string UserName, string Password);
        public sealed record SignupRequest(string Email, string UserName,string Name, string Password);
    }
}
