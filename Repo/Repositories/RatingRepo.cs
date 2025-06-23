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
        public async Task<List<Rating>> GetAllRatingsSortByCreateDateContributedCommentEqual1Async()
        {
            return await _context.Ratings
                .Where(rating => rating.ContributedComment == "1")
                .OrderByDescending(rating => rating.CreatedAt)
                .ToListAsync();
        }
        public async Task<List<Rating>> GetAllRatingsSortByCreateDateContributedCommentEqual2Async()
        {
            return await _context.Ratings
                .Where(rating => rating.ContributedComment == "2")
                .OrderByDescending(rating => rating.CreatedAt)
                .ToListAsync();
        }
        public async Task<Boolean> CheckIfUserHaveContributedCommentEqual1OrNot(User user)
        {
            return await _context.Ratings
                .Where(rating => rating.ReviewerId == user.Id && rating.ContributedComment == "1")
                .AnyAsync();
        }
        public async Task<Boolean> CheckIfUserHaveContributedCommentEqual2OrNot(User user)
        {
            return await _context.Ratings
                .Where(rating => rating.ReviewerId == user.Id && rating.ContributedComment == "2")
                .AnyAsync();
        }
    }
}
