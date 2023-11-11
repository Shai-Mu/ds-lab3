using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rsoi.Lab3.RatingService.Core;
using Rsoi.Lab3.RatingService.Dto.Models;

namespace Rsoi.Lab3.RatingService.HttpApi.Controllers;

public class RatingsController : ControllerBase
{
    private readonly IRatingRepository _ratingRepository;

    public RatingsController(IRatingRepository ratingRepository)
    {
        _ratingRepository = ratingRepository;
    }
    
    [HttpGet]
    [Route("ratings")]
    public async Task<IActionResult> FindRatingByNameAsync([FromQuery]string username)
    {
        var rating = await _ratingRepository.FindRatingForUsernameAsync(username);

        return Ok(new RatingResponse(rating));
    }

    [HttpPost]
    [Route("ratings")]
    public async Task<IActionResult> CreateRating([FromBody]CreateRatingRequest createRatingRequest)
    {
        var ratingId = await _ratingRepository.CreateRatingForUserAsync(createRatingRequest.Username, createRatingRequest.Stars);

        return Ok(ratingId);
    }

    [HttpPatch]
    [Route("ratings/modify")]
    public async Task<IActionResult> EditRatingForUser([FromQuery] string username, [FromQuery]int stars)
    {
        var rating = await _ratingRepository.FindRatingForUsernameAsync(username);

        var newStarsValue = rating!.Stars + stars;
        
        int normalizedStars = newStarsValue switch
        {
            > 100 => 100,
            < 1 => 1,
            _ => newStarsValue
        };

        await _ratingRepository.EditRatingAsync(rating.Id, normalizedStars);

        return Ok();
    }
}