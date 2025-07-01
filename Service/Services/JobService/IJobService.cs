using Repo.Entities;
using Service.DTO;
using static Service.Services.JobService.JobService;

namespace Service.Services.JobService
{
    public interface IJobService
    {
        Task AddJobAsync(CreateJobDTO jobDto, Guid? recruiterId, Guid? userId);
        Task<List<Job>> GetAllJobsAsync();
        Task<List<JobWithCompanyLogoDto>> GetAllJobsWithCompanyLogoAsync();
        Task<Job> GetJobByIdAsync(Guid jobId);
        Task<List<Job>> GetJobsByRecruiterIdAsync(Guid recruiterId);
    }
}