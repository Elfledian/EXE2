using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repo.Data;
using Repo.Entities;
using Service.Services.RecruiterService;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FastWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly TheShineDbContext _context;
        private readonly IRecruiterService _recruiterService;

        public CompanyController(TheShineDbContext context, IRecruiterService recruiterService)
        {
            _context = context;
            _recruiterService = recruiterService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllCompanies()
        {
            var companies = await _context.Companies.ToListAsync();
            return Ok(companies);
        }
        public class createCompany
        {
            public string name { get; set; }
            public string link { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult> CreateCompany(createCompany hehe)
        {
            if (string.IsNullOrEmpty(hehe.name) || string.IsNullOrEmpty(hehe.link))
            {
                return BadRequest("Name and link are required.");
            }
            if (User?.Identity?.IsAuthenticated != true || User.FindFirst(ClaimTypes.NameIdentifier) == null)
            {
                return Unauthorized(new { Message = "Authentication required or invalid token." });
            }

            Guid userId;
            try
            {
                userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User ID claim not found."));
            }
            catch (FormatException)
            {
                return Unauthorized(new { Message = "Invalid user ID format in token." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }


            var company = new Company
            {
                CompanyName = hehe.name,
                Website = hehe.link,
                CreatedAt = DateTime.UtcNow,
                RecruiterId = userId 
            };
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAllCompanies), new { id = company.CompanyId }, company);
        }
    }
}
