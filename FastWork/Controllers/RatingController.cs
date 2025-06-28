using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repo.Entities;
using Service.DTO;
using Service.Services.RatingService;

namespace FastWork.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : Controller
    {
        private readonly IRatingService _ratingService;
        private readonly UserManager<User> _userManager;
        public RatingController(IRatingService ratingService, UserManager<User> userManager)
        {
            _ratingService = ratingService ?? throw new ArgumentNullException(nameof(ratingService));
            _userManager = userManager;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddRating([FromBody] RatingDtoCreate rating)
        {
            if (rating == null)
            {
                return BadRequest("Rating data is required.");
            }
            var user = _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value).Result;
            if (user == null)
            {
                return Unauthorized("User not authenticated.");
            }
            await _ratingService.AddRatingAsync(rating, user.Id);
            return Ok("Rating added successfully.");
        }
        [HttpDelete("{ratingId}")]
        public async Task<IActionResult> DeleteRating(Guid ratingId)
        {
            await _ratingService.DeleteRatingAsync(ratingId);
            return Ok("Rating deleted successfully.");
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllRatings()
        {
            var ratings = await _ratingService.GetAllRatingsSortByCreateDateAsync();
            return Ok(ratings);
        }
        [HttpGet("{ratingId}")]
        public async Task<IActionResult> GetRatingById(Guid ratingId)
        {
            var rating = await _ratingService.GetRatingByIdAsync(ratingId);
            if (rating == null)
            {
                return NotFound("Rating not found.");
            }
            return Ok(rating);
        }
        [HttpGet("user")]
        public async Task<IActionResult> GetRatingsByUser()
        {
            var user = _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value).Result;
            if (user == null)
            {
                return Unauthorized("User not authenticated.");
            }
            var ratings = await _ratingService.GetRatingsByUserIdAsync(user.Id);
            return Ok(ratings);
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateRating([FromBody] RatingDto rating)
        {
            if (rating == null)
            {
                return BadRequest("Rating data is required.");
            }
            var user = _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value).Result;
            if (user == null)
            {
                return Unauthorized("User not authenticated.");
            }
            await _ratingService.UpdateRatingAsync(rating, user.Id);
            return Ok("Rating updated successfully.");
        }
        [HttpGet("1")]
        public async Task<IActionResult> GetRatingsByContributed1()
        {
            var ratings = await _ratingService.GetRatingsByContributed1();
            return Ok(ratings);
        }
        [HttpGet("2")]
        public async Task<IActionResult> GetRatingsByContributed2()
        {
            var ratings = await _ratingService.GetRatingsByContributed2();
            return Ok(ratings);
        }
        [HttpGet("check-contributed-1")]
        public async Task<IActionResult> CheckIfUserHaveContributedCommentEqual1()
        {
            var user = _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value).Result;
            if (user == null)
            {
                return Unauthorized("User not authenticated.");
            }
            var result = await _ratingService.CheckIfUserHaveContributedCommentEqual1OrNot(user);
            return Ok(result);
        }
        [HttpGet("check-contributed-2")]
        public async Task<IActionResult> CheckIfUserHaveContributedCommentEqual2()
        {
            var user = _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value).Result;
            if (user == null)
            {
                return Unauthorized("User not authenticated.");
            }
            var result = await _ratingService.CheckIfUserHaveContributedCommentEqual2OrNot(user);
            return Ok(result);
        }
        [HttpGet("pie-chart-all")]
        public async Task<IActionResult> GetRatingPieChart()
        {
            var pieChartData = await _ratingService.GetRatingPieChartAsync();
            return Ok(pieChartData);
        }
        [HttpGet("pie-chart1")]
        public async Task<IActionResult> GetRatingPieChart1()
        {
            var pieChartData = await _ratingService.GetRatingPieChartAsync1();
            return Ok(pieChartData);
        }
        [HttpGet("pie-chart2")]
        public async Task<IActionResult> GetRatingPieChart2()
        {
            var pieChartData = await _ratingService.GetRatingPieChartAsync2();
            return Ok(pieChartData);
        }
    }
}
