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
using Service.DTO;
using static Repo.Repositories.RatingRepo;

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
            if (rating.ContributedComment != "1" && rating.ContributedComment != "2")
                throw new ArgumentException("ContributedComment must be either '1' or '2'.", nameof(rating.ContributedComment));
            if (rating.Rating1 < 1 || rating.Rating1 > 5)
                throw new ArgumentOutOfRangeException(nameof(rating.Rating1), "Rating must be between 1 and 5.");
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null) throw new ArgumentNullException(nameof(user), "User not found.");
            if (await ratingRepo.CheckIfUserHaveContributedCommentEqual1OrNot(user) && rating.ContributedComment == "1")
                throw new InvalidOperationException("User has already contributed a comment with value '1'.");
            if (await ratingRepo.CheckIfUserHaveContributedCommentEqual2OrNot(user) && rating.ContributedComment == "2")
                throw new InvalidOperationException("User has already contributed a comment with value '2'.");
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
        public async Task<List<RatingPieChartDto>> GetRatingPieChartAsync()
        {
            var ratings = await ratingRepo.GetRatingPieChartAsync();
            if (ratings == null || !ratings.Any())
            {
                // Return 0% for all stars if no ratings
                return Enumerable.Range(1, 5).Select(i => new RatingPieChartDto
                {
                    Star = i,
                    Percentage = 0
                }).ToList();
            }
            return ratings;
        }
        public async Task<List<RatingPieChartDto>> GetRatingPieChartAsync1()
        {
            var ratings = await ratingRepo.GetRatingPieChartAsync1();
            if (ratings == null || !ratings.Any())
            {
                // Return 0% for all stars if no ratings
                return Enumerable.Range(1, 5).Select(i => new RatingPieChartDto
                {
                    Star = i,
                    Percentage = 0
                }).ToList();
            }
            return ratings;
        }
        public async Task<List<RatingPieChartDto>> GetRatingPieChartAsync2()
        {
            var ratings = await ratingRepo.GetRatingPieChartAsync2();
            if (ratings == null || !ratings.Any())
            {
                // Return 0% for all stars if no ratings
                return Enumerable.Range(1, 5).Select(i => new RatingPieChartDto
                {
                    Star = i,
                    Percentage = 0
                }).ToList();
            }
            return ratings;
        }
    }
}