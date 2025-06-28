using Repo.Entities;
using Service.DTO;
using static Repo.Repositories.RatingRepo;

namespace Service.Services.RatingService
{
    public interface IRatingService
    {
        Task AddRatingAsync(RatingDtoCreate rating, Guid userId);
        Task DeleteRatingAsync(Guid ratingId);
        Task<List<Rating>> GetAllRatingsSortByCreateDateAsync();
        Task<Rating> GetRatingByIdAsync(Guid ratingId);
        Task<List<Rating>> GetRatingsByUserIdAsync(Guid userId);
        Task UpdateRatingAsync(RatingDto rating, Guid userId);
        Task<bool> CheckIfUserHaveContributedCommentEqual1OrNot(User user);
        Task<bool> CheckIfUserHaveContributedCommentEqual2OrNot(User user);
        Task<List<Rating>> GetRatingsByContributed1();
        Task<List<Rating>> GetRatingsByContributed2();
        Task<List<RatingPieChartDto>> GetRatingPieChartAsync();
        Task<List<RatingPieChartDto>> GetRatingPieChartAsync1();
        Task<List<RatingPieChartDto>> GetRatingPieChartAsync2();
    }
}