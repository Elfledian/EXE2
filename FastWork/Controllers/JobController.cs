using Microsoft.AspNetCore.Mvc;
using Repo.Entities;
using Service.DTO;
using Service.Services.JobService;
using Service.Services.RecruiterService;
using System.Security.Claims;

namespace FastWork.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly IRecruiterService _recruiterService;
        public JobController(IJobService jobService, IRecruiterService recruiterService)
        {
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _recruiterService = recruiterService ?? throw new ArgumentNullException(nameof(recruiterService));
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var jobs = await _jobService.GetAllJobsAsync();
            return Ok(jobs);
        }
        [HttpGet("GetAllWithImg")]
        public async Task<IActionResult> GetAllWithImage()
        {
            var jobs = await _jobService.GetAllJobsWithCompanyLogoAsync();
            return Ok(jobs);
        }
        [HttpGet("{jobId}")]
        public async Task<IActionResult> GetById(Guid jobId)
        {
            var job = await _jobService.GetJobByIdAsync(jobId);
            if (job == null)
            {
                return NotFound();
            }
            return Ok(job);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateJobDTO jobDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state.", Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            // Check if user is authenticated and has the required claim
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

            // Retrieve recruiter by user ID
            var recruiter = await _recruiterService.GetByUserId(userId);
            if (recruiter == null)
            {
                return NotFound(new { Message = "Recruiter not found for the authenticated user." });
            }

            try
            {
                await _jobService.AddJobAsync(jobDto, recruiter.RecruiterId, userId);
                return Ok(new { Message = "Job created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the job.", Error = ex.Message });
            }
        }
        [HttpGet("GetByRecruiterId")]
        public async Task<IActionResult> GetByRecruiterId()
        {
            var userId = Guid.Parse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId == Guid.Empty)
            {
                return Unauthorized(new { Message = "User ID not found in the token." });
            }
            var recruiter = await _recruiterService.GetByUserId(userId);
            if (recruiter == null)
            {
                return NotFound(new { Message = "Recruiter not found for the authenticated user." });
            }
            var recruiterId = recruiter.RecruiterId;
            var jobs = await _jobService.GetJobsByRecruiterIdAsync(recruiterId);
            if (jobs == null || !jobs.Any())
            {
                return NotFound(new { Message = "No jobs found for the specified recruiter." });
            }
            return Ok(jobs);
        }
    }
}
