using Repo.Entities;
using Repo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.RecruiterService
{
    public class RecruiterService : IRecruiterService
    {
        private readonly RecruiterRepo _recruiterRepo;

        public RecruiterService(RecruiterRepo recruiterRepo)
        {
            _recruiterRepo = recruiterRepo ?? throw new ArgumentNullException(nameof(recruiterRepo));
        }

        public async Task<Recruiter> GetByUserId(Guid userId)
        {
            return await _recruiterRepo.GetByUserId(userId);
        }
    }
}
