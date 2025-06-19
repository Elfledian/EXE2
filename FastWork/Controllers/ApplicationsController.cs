using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repo.Data;
using Repo.Entities;
using Service.Services.ApplicationsService;

namespace FastWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationServices _applicationServices;
        private readonly UserManager<User> _userManager;
        private readonly TheShineDbContext _context;
        public ApplicationsController(IApplicationServices applicationServices, UserManager<User> userManager, TheShineDbContext context)
        {
            _applicationServices = applicationServices ?? throw new ArgumentNullException(nameof(applicationServices));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        [Authorize]
        [HttpPost("apply/{jobId}")]
        public async Task<IActionResult> ApplyAsync(Guid jobId, [FromBody] ApplyRequest request)
        {
            var user = _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value).Result;
            if (user == null)
            {
                return Unauthorized("User not authenticated.");
            }
            if (request == null || string.IsNullOrEmpty(request.About) || string.IsNullOrEmpty(request.AboutMe))
            {
                return BadRequest("Invalid application data.");
            }
            var result = await _applicationServices.ApplyAsync(jobId, user, request.About, request.AboutMe);
            return result;
        }
        [HttpGet("applications/{jobId}")]
        public async Task<IActionResult> GetApplicationsByJobIdAsync(Guid jobId)
        {
            var applications = await _applicationServices.GetApplicationsByJobIdAsync(jobId);
            if (applications == null || !applications.Any())
            {
                return NotFound("No applications found for this job.");
            }
            return Ok(applications);
        }
        [HttpGet("candidate/self")]
        public async Task<IActionResult> GetApplicationsByCandidateIdAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }
            var candidate = _context.Candidates.FirstOrDefault(c => c.UserId == Guid.Parse(userId));
            if (candidate == null)
            {
                return NotFound("Candidate not found.");
            }
            var applications = await _applicationServices.GetApplicationsByCandidateIdAsync(candidate.CandidateId);
            if (applications == null || !applications.Any())
            {
                return NotFound("No applications found for this candidate.");
            }
            return Ok(applications);
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllApplicationsAsync()
        {
            var applications = await _applicationServices.GetAllApplicationsAsync();
            if (applications == null || !applications.Any())
            {
                return NotFound("No applications found.");
            }
            return Ok(applications);
        }
        [HttpGet("{applicationId}")]
        public async Task<IActionResult> GetApplicationByIdAsync(Guid applicationId)
        {
            var application = await _applicationServices.GetApplicationByIdAsync(applicationId);
            if (application == null)
            {
                return NotFound("Application not found.");
            }
            return Ok(application);
        }

        [HttpPut("{applicationId}")]
        public async Task<IActionResult> UpdateApplicationAsync(Guid applicationId, [FromBody] Application application)
        {
            if (application == null || application.ApplicationId != applicationId)
            {
                return BadRequest("Invalid application data.");
            }
            await _applicationServices.UpdateApplicationAsync(application);
            return NoContent();
        }
        [HttpDelete("{applicationId}")]
        public async Task<IActionResult> DeleteApplicationAsync(Guid applicationId)
        {
            await _applicationServices.DeleteApplicationAsync(applicationId);
            return NoContent();
        }
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetApplicationsByStatusAsync(string status)
        {
            var applications = await _applicationServices.GetApplicationsByStatusAsync(status);
            if (applications == null || !applications.Any())
            {
                return NotFound("No applications found with the specified status.");
            }
            return Ok(applications);
        }
        [HttpGet("applications/count/{jobId}")]
        public async Task<IActionResult> GetAmmountOfApplicationsByJobIdAsync(Guid jobId)
        {
            var count = await _applicationServices.GetAmmountOfApplicationsByJobIdAsync(jobId);
            var result = "There are " + count + " applications for this job.";
            return Ok(result);
        }
        public class ApplyRequest
        {
            public string About { get; set; }
            public string AboutMe { get; set; }
        }
    }
}
