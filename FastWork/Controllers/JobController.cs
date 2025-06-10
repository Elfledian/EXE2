using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Services.JobService;
using System.Security.Claims;

namespace FastWork.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;
        public JobController(IJobService jobService)
        {
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var jobs = await _jobService.GetAllJobsAsync();
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
        public async Task<IActionResult> Create([FromBody]CreateJobDTO jobDto)
        {
            if (jobDto == null)
            {
                return BadRequest("Job data is null.");
            }
            Guid recruiterId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (ModelState.IsValid)
            {
                await _jobService.AddJobAsync(jobDto,recruiterId);
                return RedirectToAction("GetAll");
            }
            return BadRequest(new
            {
                Message = "Model state is invalid.",
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }
    }
}
