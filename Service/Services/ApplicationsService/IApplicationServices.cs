using Microsoft.AspNetCore.Mvc;
using Repo.Entities;

namespace Service.Services.ApplicationsService
{
    public interface IApplicationServices
    {
        Task<IActionResult> ApplyAsync(Guid jobId, User user, string? about, string? aboutMe);
        Task DeleteApplicationAsync(Guid applicationId);
        Task<List<Application>> GetAllApplicationsAsync();
        Task<Application?> GetApplicationByIdAsync(Guid applicationId);
        Task<List<Application>> GetApplicationsByCandidateIdAsync(Guid candidateId);
        Task<List<Application>> GetApplicationsByJobIdAsync(Guid jobId);
        Task<List<Application>> GetApplicationsByStatusAsync(string status);
        Task UpdateApplicationAsync(Application application);
        Task<int> GetAmmountOfApplicationsByJobIdAsync(Guid jobId);
    }
}