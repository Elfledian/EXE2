using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repo.Entities;
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
        public async Task<IActionResult> AddRating([FromBody] RatingService.RatingDto rating)
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
        public async Task<IActionResult> UpdateRating([FromBody] RatingService.RatingDto rating)
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
    }
}
