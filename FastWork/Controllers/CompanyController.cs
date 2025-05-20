using FastWork.Data;
using FastWork.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FastWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly TheShineDbContext _context;

        public CompanyController(TheShineDbContext context)
        {
            _context = context;
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
            var company = new Company
            {
                CompanyName = hehe.name,
                Website = hehe.link,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAllCompanies), new { id = company.CompanyId }, company);
        }
    }
}
