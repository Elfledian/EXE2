﻿
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repo.Entities;
using Repo.Repositories;
using Service.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.JobService
{
    public class JobService : IJobService
    {
        private readonly JobRepo _jobRepo;
        private readonly RecruiterRepo _recruiterRepo;
        private readonly UserManager<User> _userManager;

        public JobService(JobRepo jobRepo, RecruiterRepo recruiterRepo)
        {
            _jobRepo = jobRepo ?? throw new ArgumentNullException(nameof(jobRepo));
            _recruiterRepo = recruiterRepo ?? throw new ArgumentNullException(nameof(recruiterRepo));
        }
        public async Task<List<Job>> GetAllJobsAsync()
        {
            return await _jobRepo.GetAll().ToListAsync();
        }
        public class JobWithCompanyLogoDto
        {
            public Job Job { get; set; }
            public Guid? LogoFileId { get; set; }
        }
        public async Task<List<JobWithCompanyLogoDto>> GetAllJobsWithCompanyLogoAsync()
        {
            // Ensure Company is included for each Job
            var jobs = await _jobRepo.GetAll()
                .Include(j => j.Company)
                .Select(j => new JobWithCompanyLogoDto
                {
                    Job = j,
                    LogoFileId = j.Company.LogoFileId
                })
                .ToListAsync();

            return jobs;
        }
        public async Task<Job> GetJobByIdAsync(Guid jobId)
        {
            return await _jobRepo.GetEntityByIdAsync(jobId);
        }

        public async Task AddJobAsync(CreateJobDTO jobDto, Guid? recruitetId, Guid? userId)
        {
            var company = await _recruiterRepo.GetCompanyAsyncof(userId);
            var job = new Job
            {
                JobId = Guid.NewGuid(),
                Title = jobDto.Title,
                CompanyName = company.CompanyName,
                Description = jobDto.Description,
                JobDetails = jobDto.JobDetails,
                Requirements = jobDto.Requirements,
                Experience = jobDto.Experience,
                Benefits = jobDto.Benefits,
                Salary = jobDto.Salary,
                Position = jobDto.Position,
                PostedAt = DateTime.UtcNow,
                CategoryId = jobDto.CategoryId,
                RecruiterId = recruitetId,
                IsUrgent = jobDto.IsUrgent ?? false,
                ContactPhone = jobDto.ContactPhone,
                Company = company,
                Recruiter = _recruiterRepo.GetById((Guid)recruitetId) ?? throw new ArgumentNullException(nameof(recruitetId)),
            };

            if (job == null) throw new ArgumentNullException(nameof(job));
            await _jobRepo.AddAsync(job);
            await _jobRepo.SaveChangesAsync();
        }
        public async Task<List<Job>> GetJobsByRecruiterIdAsync(Guid recruiterId)
        {
            return await _jobRepo.GetJobsByRecruiterIdAsync(recruiterId);
        }
    }
}
