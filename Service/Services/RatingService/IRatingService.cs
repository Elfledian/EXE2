using Repo.Entities;

namespace Service.Services.RatingService
{
    public interface IRatingService
    {
        Task AddRatingAsync(RatingService.RatingDtoCreate rating, Guid userId);
        Task DeleteRatingAsync(Guid ratingId);
        Task<List<Rating>> GetAllRatingsSortByCreateDateAsync();
        Task<Rating> GetRatingByIdAsync(Guid ratingId);
        Task<List<Rating>> GetRatingsByUserIdAsync(Guid userId);
        Task UpdateRatingAsync(RatingService.RatingDto rating, Guid userId);
        Task<bool> CheckIfUserHaveContributedCommentEqual1OrNot(User user);
        Task<bool> CheckIfUserHaveContributedCommentEqual2OrNot(User user);
        Task<List<Rating>> GetRatingsByContributed1();
        Task<List<Rating>> GetRatingsByContributed2();
    }
}