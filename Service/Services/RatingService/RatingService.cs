using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repo.Entities;
using Repo.Repositories;

namespace Service.Services.RatingService
{
    public class RatingService : IRatingService
    {
        private readonly RatingRepo ratingRepo;
        private UserManager<User> userManager;
        public RatingService(RatingRepo ratingRepo, UserManager<User> userManager)
        {
            this.ratingRepo = ratingRepo ?? throw new ArgumentNullException(nameof(ratingRepo));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
        public async Task<List<Rating>> GetAllRatingsSortByCreateDateAsync()
        {
            return await ratingRepo.GetAll()
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
        public async Task<Rating> GetRatingByIdAsync(Guid ratingId)
        {
            return await ratingRepo.GetEntityByIdAsync(ratingId);
        }
        public async Task<List<Rating>> GetRatingsByUserIdAsync(Guid userId)
        {
            return await ratingRepo.GetRatingsByReviewerIdAsync(userId);
        }
        public async Task AddRatingAsync(RatingDtoCreate rating, Guid userId)
        {
            if (rating == null) throw new ArgumentNullException(nameof(rating));
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null) throw new ArgumentNullException(nameof(user), "User not found.");
            var ratingEntity = new Rating
            {
                RatingId = Guid.NewGuid(),
                Rating1 = rating.Rating1,
                Comment = rating.Comment,
                ContributedComment = rating.ContributedComment,
                ReviewerId = userId,
                Reviewer = user,
                CreatedAt = DateTime.UtcNow
            };
            await ratingRepo.AddAsync(ratingEntity);
            await ratingRepo.SaveChangesAsync();
        }
        public async Task UpdateRatingAsync(RatingDto rating, Guid userId)
        {
            var existingRating = await GetRatingByIdAsync(rating.RatingId);
            if (existingRating == null) throw new ArgumentNullException(nameof(existingRating), "Rating not found.");

            ratingRepo.Update(existingRating);
            await ratingRepo.SaveChangesAsync();
        }

        public async Task DeleteRatingAsync(Guid ratingId)
        {
            var rating = await GetRatingByIdAsync(ratingId);
            if (rating == null) throw new ArgumentNullException(nameof(rating));
            ratingRepo.Delete(rating);
            await ratingRepo.SaveChangesAsync();
        }
        public async Task<bool> CheckIfUserHaveContributedCommentEqual1OrNot(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user), "User not found.");
            return await ratingRepo.CheckIfUserHaveContributedCommentEqual1OrNot(user);
        }
        public async Task<bool> CheckIfUserHaveContributedCommentEqual2OrNot(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user), "User not found.");
            return await ratingRepo.CheckIfUserHaveContributedCommentEqual2OrNot(user);
        }
        public async Task<List<Rating>> GetRatingsByContributed1()
        {
            return await ratingRepo.GetAllRatingsSortByCreateDateContributedCommentEqual1Async();
        }
        public async Task<List<Rating>> GetRatingsByContributed2()
        {
            return await ratingRepo.GetAllRatingsSortByCreateDateContributedCommentEqual2Async();
        }
        public class RatingDto
        {
            public Guid RatingId { get; set; }
            public int? Rating1 { get; set; }
            public string Comment { get; set; }
            public string ContributedComment { get; set; }
        }
        public class RatingDtoCreate
        {
            public int? Rating1 { get; set; }
            public string Comment { get; set; }
            public string ContributedComment { get; set; }
        }

    }
}