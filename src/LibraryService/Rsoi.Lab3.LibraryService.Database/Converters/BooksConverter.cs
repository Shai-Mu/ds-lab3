using System.Diagnostics.CodeAnalysis;
using Rsoi.Lab3.LibraryService.Database.Converters.EnumConverters;
using Rsoi.Lab3.LibraryService.Database.Models;
using DbBooks = Rsoi.Lab3.LibraryService.Database.Models.Books;
using CoreBooks = Rsoi.Lab3.LibraryService.Core.Models.Books;

namespace Rsoi.Lab3.LibraryService.Database.Converters;

public static class BooksConverter
{
    [return: NotNullIfNotNull("dbBooks")]
    public static CoreBooks? Convert(Books? dbBooks)
    {
        if (dbBooks is null)
            return null;

        return new CoreBooks(dbBooks.Id,
            dbBooks.Name,
            dbBooks.Author,
            dbBooks.Genre,
            BookConditionConverter.Convert(dbBooks.Condition));
    }
}