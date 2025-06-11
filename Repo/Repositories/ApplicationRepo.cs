using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repo.Data;
using Repo.Entities;

namespace Repo.Repositories
{
    public class ApplicationRepo
    {
        private readonly TheShineDbContext _context;
        private readonly UserManager<User> _userManager;

        public ApplicationRepo()
        {
            _context = new TheShineDbContext();
        }
        public ApplicationRepo(TheShineDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Apply(Guid jobid, User user, string? about, string? aboutMe)
        {
            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == jobid);
            if (job == null)
            {
                return new NotFoundResult(); // Job not found
            }
            var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.UserId == user.Id);
            var application = new Application
            {
                JobId = jobid,
                About = about ?? "Application about text",
                AboutMe = aboutMe ?? "About me text",
                ApplicationId = Guid.NewGuid(),
                CandidateId = user.Id,
                Status = "Applying",
                InterviewType = "Online", // Default interview type, can be changed later
                AppliedAt = DateTime.UtcNow,
                Candidate = candidate,
                Job = job
            };
            if (candidate == null)
            {
                return new NotFoundResult(); // Candidate not found
            }
            if (job.Applications.Any(a => a.CandidateId == candidate.CandidateId))
            {
                return new BadRequestObjectResult("You have already applied for this job.");
            }
            _context.Applications.Add(application);
            try
            {
                await _context.SaveChangesAsync();
                return new OkObjectResult(application); // Return the created application
            }
            catch (DbUpdateException ex)
            {
                // Log the exception (ex) here if needed
                return new StatusCodeResult(500); // Internal server error
            }
        }
        public async Task<List<Application>> GetApplicationsByCandidateIdAsync(Guid candidateId)
        {
            return await _context.Applications
                .Where(a => a.CandidateId == candidateId)
                .Include(a => a.Job)
                .ToListAsync();
        }
        public async Task<List<Application>> GetApplicationsByJobIdAsync(Guid jobId)
        {
            return await _context.Applications
                .Where(a => a.JobId == jobId)
                .Include(a => a.Candidate)
                .ToListAsync();
        }
        public async Task<Application> GetApplicationByIdAsync(Guid applicationId)
        {
            return await _context.Applications
                .Include(a => a.Candidate)
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
        }
        public async Task UpdateApplicationAsync(Application application)
        {
            _context.Applications.Update(application);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteApplicationAsync(Guid applicationId)
        {
            var application = await GetApplicationByIdAsync(applicationId);
            if (application != null)
            {
                _context.Applications.Remove(application);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Application>> GetAllApplicationsAsync()
        {
            return await _context.Applications
                .Include(a => a.Candidate)
                .Include(a => a.Job)
                .ToListAsync();
        }
        public async Task<List<Application>> GetApplicationsByStatusAsync(string status)
        {
            return await _context.Applications
                .Where(a => a.Status == status)
                .Include(a => a.Candidate)
                .Include(a => a.Job)
                .ToListAsync();
        }
        public async Task<List<Application>> GetApplicationsByInterviewTypeAsync(string interviewType)
        {
            return await _context.Applications
                .Where(a => a.InterviewType == interviewType)
                .Include(a => a.Candidate)
                .Include(a => a.Job)
                .ToListAsync();
        }
        public async Task<List<Application>> GetApplicationsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Applications
                .Where(a => a.AppliedAt >= startDate && a.AppliedAt <= endDate)
                .Include(a => a.Candidate)
                .Include(a => a.Job)
                .ToListAsync();
        }
        
    }
}
