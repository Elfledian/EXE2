using Repo.Entities;
using Service.DTO;

namespace Service.Services.JobService
{
    public interface IJobService
    {
        Task AddJobAsync(CreateJobDTO jobDto, Guid? recruiterId, Guid? userId);
        Task<List<Job>> GetAllJobsAsync();
        Task<Job> GetJobByIdAsync(Guid jobId);
        Task<List<Job>> GetJobsByRecruiterIdAsync(Guid recruiterId);
    }
}