using Microsoft.EntityFrameworkCore;
using Repo.Data;
using Repo.Entities;
using Repo.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.Repositories
{
    public class JobRepo : GenericRepository<Job>
    {
        public JobRepo(TheShineDbContext context) : base(context)
        {
        }
        public async Task<List<Job>> GetJobsByRecruiterIdAsync(Guid recruiterId)
        {
            return await _context.Jobs
                .Where(job => job.RecruiterId == recruiterId)
                .ToListAsync();
        }
    }
}
