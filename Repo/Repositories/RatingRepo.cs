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
        public async Task<List<RatingPieChartDto>> GetRatingPieChartAsync()
        {
            var ratings = await _context.Set<Rating>()
                .Where(r => r.Rating1.HasValue)
                .ToListAsync();

            int total = ratings.Count;
            if (total == 0)
            {
                // Return 0% for all stars if no ratings
                return Enumerable.Range(1, 5)
                    .Select(star => new RatingPieChartDto
                    {
                        Star = star,
                        Count = 0,
                        Percentage = 0
                    }).ToList();
            }

            var result = ratings
                .GroupBy(r => r.Rating1.Value)
                .Select(g => new
                {
                    Star = g.Key,
                    Count = g.Count()
                })
                .ToList();

            var pieChart = new List<RatingPieChartDto>();
            double sumPercent = 0;
            int sumCount = 0;

            // Calculate for stars 1-4
            for (int star = 1; star <= 4; star++)
            {
                int count = result.FirstOrDefault(x => x.Star == star)?.Count ?? 0;
                double percent = Math.Round((double)count / total * 100, 2);
                sumPercent += percent;
                sumCount += count;
                pieChart.Add(new RatingPieChartDto
                {
                    Star = star,
                    Count = count,
                    Percentage = percent
                });
            }

            // Star 5: ensure total is exactly 100%
            int count5 = result.FirstOrDefault(x => x.Star == 5)?.Count ?? 0;
            double percent5 = Math.Round(100 - sumPercent, 2);
            // If there are no 5-star ratings, set to 0
            if (count5 == 0) percent5 = 0;
            pieChart.Add(new RatingPieChartDto
            {
                Star = 5,
                Count = count5,
                Percentage = percent5
            });

            return pieChart;
        }
        public async Task<List<RatingPieChartDto>> GetRatingPieChartAsync1()
        {
            var ratings = await _context.Set<Rating>()
                .Where(r => r.Rating1.HasValue && r.ContributedComment.Equals("1"))
                .ToListAsync();

            int total = ratings.Count;
            if (total == 0)
            {
                // Return 0% for all stars if no ratings
                return Enumerable.Range(1, 5)
                    .Select(star => new RatingPieChartDto
                    {
                        Star = star,
                        Count = 0,
                        Percentage = 0
                    }).ToList();
            }

            var result = ratings
                .GroupBy(r => r.Rating1.Value)
                .Select(g => new
                {
                    Star = g.Key,
                    Count = g.Count()
                })
                .ToList();

            var pieChart = new List<RatingPieChartDto>();
            double sumPercent = 0;
            int sumCount = 0;

            // Calculate for stars 1-4
            for (int star = 1; star <= 4; star++)
            {
                int count = result.FirstOrDefault(x => x.Star == star)?.Count ?? 0;
                double percent = Math.Round((double)count / total * 100, 2);
                sumPercent += percent;
                sumCount += count;
                pieChart.Add(new RatingPieChartDto
                {
                    Star = star,
                    Count = count,
                    Percentage = percent
                });
            }

            // Star 5: ensure total is exactly 100%
            int count5 = result.FirstOrDefault(x => x.Star == 5)?.Count ?? 0;
            double percent5 = Math.Round(100 - sumPercent, 2);
            // If there are no 5-star ratings, set to 0
            if (count5 == 0) percent5 = 0;
            pieChart.Add(new RatingPieChartDto
            {
                Star = 5,
                Count = count5,
                Percentage = percent5
            });

            return pieChart;
        }
        public async Task<List<RatingPieChartDto>> GetRatingPieChartAsync2()
        {
            var ratings = await _context.Set<Rating>()
                .Where(r => r.Rating1.HasValue && r.ContributedComment.Equals("2"))
                .ToListAsync();

            int total = ratings.Count;
            if (total == 0)
            {
                // Return 0% for all stars if no ratings
                return Enumerable.Range(1, 5)
                    .Select(star => new RatingPieChartDto
                    {
                        Star = star,
                        Count = 0,
                        Percentage = 0
                    }).ToList();
            }

            var result = ratings
                .GroupBy(r => r.Rating1.Value)
                .Select(g => new
                {
                    Star = g.Key,
                    Count = g.Count()
                })
                .ToList();

            var pieChart = new List<RatingPieChartDto>();
            double sumPercent = 0;
            int sumCount = 0;

            // Calculate for stars 1-4
            for (int star = 1; star <= 4; star++)
            {
                int count = result.FirstOrDefault(x => x.Star == star)?.Count ?? 0;
                double percent = Math.Round((double)count / total * 100, 2);
                sumPercent += percent;
                sumCount += count;
                pieChart.Add(new RatingPieChartDto
                {
                    Star = star,
                    Count = count,
                    Percentage = percent
                });
            }

            // Star 5: ensure total is exactly 100%
            int count5 = result.FirstOrDefault(x => x.Star == 5)?.Count ?? 0;
            double percent5 = Math.Round(100 - sumPercent, 2);
            // If there are no 5-star ratings, set to 0
            if (count5 == 0) percent5 = 0;
            pieChart.Add(new RatingPieChartDto
            {
                Star = 5,
                Count = count5,
                Percentage = percent5
            });

            return pieChart;
        }
        public class RatingPieChartDto
        {
            public int Star { get; set; }         // 1-5
            public int Count { get; set; }        // Number of ratings for this star
            public double Percentage { get; set; } // Percentage of total ratings
        }
    }
}
