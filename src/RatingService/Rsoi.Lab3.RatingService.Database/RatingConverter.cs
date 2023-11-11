using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using DbRating = Rsoi.Lab3.RatingService.Database.Rating;
using CoreRating = Rsoi.Lab3.RatingService.Core.Rating;

namespace Rsoi.Lab3.RatingService.Database;

public static class RatingConverter
{
    [return: NotNullIfNotNull("dbRating")]
    public static CoreRating? Convert(Rating? dbRating)
    {
        if (dbRating is null)
            return null;

        return new CoreRating(dbRating.Id, dbRating.Username, dbRating.Stars);
    }
}