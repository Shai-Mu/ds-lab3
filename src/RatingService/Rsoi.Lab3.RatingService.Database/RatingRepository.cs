using Microsoft.EntityFrameworkCore;
using Rsoi.Lab3.RatingService.Core;

namespace Rsoi.Lab3.RatingService.Database;

public class RatingRepository : IRatingRepository
{
    private readonly RatingContext _ratingContext;

    public RatingRepository(RatingContext ratingContext)
    {
        _ratingContext = ratingContext;
    }

    public async Task<Guid> CreateRatingForUserAsync(string username, int stars)
    {
        var rating = new Rating(Guid.NewGuid(), username, stars);

        if (await _ratingContext.Ratings.AnyAsync(r => r.Username == username))
            throw new Exception($"Entity with name {username} exists.");

        _ratingContext.Ratings.Add(rating);

        await _ratingContext.SaveChangesAsync();

        return rating.Id;
    }

    public async Task<Lab3.RatingService.Core.Rating?> FindRatingForUsernameAsync(string username)
    {
        var rating = await _ratingContext.Ratings
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Username == username);

        return RatingConverter.Convert(rating);
    }

    public async Task EditRatingAsync(Guid id, int stars)
    {
        var rating = await _ratingContext.Ratings
            .FirstAsync(r => r.Id == id);

        rating.Stars = stars;

        await _ratingContext.SaveChangesAsync();
    }

    public async Task<Core.Rating> GetRatingAsync(Guid id)
    {
        var rating = await _ratingContext.Ratings
            .AsNoTracking()
            .FirstAsync(r => r.Id == id);

        return RatingConverter.Convert(rating);
    }
}