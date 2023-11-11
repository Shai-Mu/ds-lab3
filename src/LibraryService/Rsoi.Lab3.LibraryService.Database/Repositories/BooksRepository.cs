using Microsoft.EntityFrameworkCore;
using Rsoi.Lab3.LibraryService.Core.Interfaces;
using Rsoi.Lab3.LibraryService.Core.Models.Enums;
using Rsoi.Lab3.LibraryService.Database.Models;
using Rsoi.Lab3.LibraryService.Database.Converters;
using Rsoi.Lab3.LibraryService.Database.Converters.EnumConverters;
using Books = Rsoi.Lab3.LibraryService.Core.Models.Books;

namespace Rsoi.Lab3.LibraryService.Database.Repositories;

public class BooksRepository : IBooksRepository
{
    private readonly LibraryContext _libraryContext;

    public BooksRepository(LibraryContext libraryContext)
    {
        _libraryContext = libraryContext;
    }

    public async Task<Guid> CreateBooksAsync(Guid id, string name, string genre, string author, BookCondition condition)
    {
        var books = new Models.Books(id, name, author, genre, BookConditionConverter.Convert(condition));

        await _libraryContext.Books.AddAsync(books);

        await _libraryContext.SaveChangesAsync();

        return books.Id;
    }

    public async Task<Books?> FindBooksWithCredentialsAsync(string name, string genre, string author, BookCondition condition)
    {
        var books = await _libraryContext.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(b =>
                b.Author == author 
                && b.Condition == BookConditionConverter.Convert(condition)
                && b.Name == name
                && b.Genre == genre);

        return BooksConverter.Convert(books);
    }

    public async Task<Books> GetBooksAsync(Guid id)
    {
        var book = await _libraryContext.Books
            .AsNoTracking()
            .FirstAsync(b => b.Id == id);

        return BooksConverter.Convert(book);
    }
}