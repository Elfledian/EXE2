using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repo.Entities;
using Repo.Repositories;

namespace Service.Services.ApplicationsService
{
    public class ApplicationServices : IApplicationServices
    {
        private readonly ApplicationRepo _applicationRepo;
        public ApplicationServices()
        {
            _applicationRepo = new ApplicationRepo();
        }
        public ApplicationServices(ApplicationRepo applicationRepo)
        {
            _applicationRepo = applicationRepo;
        }
        public async Task<IActionResult> ApplyAsync(Guid jobId, User user, string? about, string? aboutMe)
        {
            return await _applicationRepo.Apply(jobId, user, about, aboutMe);
        }
        public async Task<List<Application>> GetApplicationsByCandidateIdAsync(Guid candidateId)
        {
            return await _applicationRepo.GetApplicationsByCandidateIdAsync(candidateId);
        }
        public async Task<List<Application>> GetApplicationsByJobIdAsync(Guid jobId)
        {
            return await _applicationRepo.GetApplicationsByJobIdAsync(jobId);
        }
        public async Task<List<Application>> GetAllApplicationsAsync()
        {
            return await _applicationRepo.GetAllApplicationsAsync();
        }
        public async Task<Application?> GetApplicationByIdAsync(Guid applicationId)
        {
            return await _applicationRepo.GetApplicationByIdAsync(applicationId);
        }
        public Task UpdateApplicationAsync(Application application)
        {
            return _applicationRepo.UpdateApplicationAsync(application);
        }
        public Task DeleteApplicationAsync(Guid applicationId)
        {
            return _applicationRepo.DeleteApplicationAsync(applicationId);
        }
        public async Task<List<Application>> GetApplicationsByStatusAsync(string status)
        {
            return await _applicationRepo.GetApplicationsByStatusAsync(status);
        }
    }
}
