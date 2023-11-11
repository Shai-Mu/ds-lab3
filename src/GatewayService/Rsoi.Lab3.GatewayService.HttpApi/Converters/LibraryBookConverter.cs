using System.Diagnostics.CodeAnalysis;
using Rsoi.Lab3.GatewayService.HttpApi.Models;
using Rsoi.Lab3.LibraryService.Dto.Models;

namespace Rsoi.Lab3.GatewayService.HttpApi.Converters;

public static class LibraryBookConverter
{
    public static LibraryBookResponse Convert(BooksWithCount booksWithCount)
    {
        return new LibraryBookResponse(booksWithCount.Id, 
            booksWithCount.Name, 
            booksWithCount.Author,
            booksWithCount.Genre, 
            BookConditionConverter.Convert(booksWithCount.Condition), 
            booksWithCount.Count);
    }

    [return: NotNullIfNotNull("books")]
    public static BookInfo? Convert(Books? books)
    {
        if (books is null)
            return null;
        
        return new BookInfo(books.Id, books.Name, books.Author, books.Genre);
    }
}