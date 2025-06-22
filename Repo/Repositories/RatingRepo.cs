using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repo.Data;
using Repo.Entities;
using Repo.GenericRepository;

namespace Repo.Repositories
{
    public class RatingRepo : GenericRepository<Rating>
    {
        public RatingRepo(TheShineDbContext context) : base(context)
        {
        }
        public async Task<List<Rating>> GetRatingsByReviewerIdAsync(Guid reviewerId)
        {
            return await _context.Ratings
                .Where(rating => rating.ReviewerId == reviewerId)
                .ToListAsync();
        }
    }
}
