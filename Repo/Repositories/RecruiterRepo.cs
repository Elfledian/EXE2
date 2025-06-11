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
    public class RecruiterRepo : GenericRepository<Recruiter>
    {
        public RecruiterRepo(TheShineDbContext context) : base(context)
        {
        }

        public async Task<Recruiter> GetByUserId(Guid userId)
        {
            return await _context.Recruiters.FirstOrDefaultAsync(r => r.UserId == userId);
        }
        public async Task<Company> GetCompanyAsyncof(Guid? recruiterId)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.RecruiterId == recruiterId);
        }
    }
}
