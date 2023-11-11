using Rsoi.Lab3.LibraryService.Database.Models.Enums;
using DbBookCondition = Rsoi.Lab3.LibraryService.Database.Models.Enums.BookCondition;
using CoreBookCondition = Rsoi.Lab3.LibraryService.Core.Models.Enums.BookCondition;

namespace Rsoi.Lab3.LibraryService.Database.Converters.EnumConverters;

public static class BookConditionConverter
{
    public static BookCondition Convert(CoreBookCondition coreBookCondition)
    {
        return coreBookCondition switch
        {
            CoreBookCondition.Bad => BookCondition.Bad,
            CoreBookCondition.Excellent => BookCondition.Excellent,
            CoreBookCondition.Good => BookCondition.Good,
            _ => throw new ArgumentOutOfRangeException(nameof(coreBookCondition), coreBookCondition, "")
        };
    }
    
    public static CoreBookCondition Convert(BookCondition coreBookCondition)
    {
        return coreBookCondition switch
        {
            BookCondition.Bad => CoreBookCondition.Bad,
            BookCondition.Excellent => CoreBookCondition.Excellent,
            BookCondition.Good => CoreBookCondition.Good,
            _ => throw new ArgumentOutOfRangeException(nameof(coreBookCondition), coreBookCondition, "")
        };
    }
}