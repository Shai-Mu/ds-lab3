using System.Diagnostics.CodeAnalysis;
using Rsoi.Lab3.LibraryService.Database.Models;
using DbLibraryBooks = Rsoi.Lab3.LibraryService.Database.Models.LibraryBooks;
using CoreLibraryBooks = Rsoi.Lab3.LibraryService.Core.Models.LibraryBooks;

namespace Rsoi.Lab3.LibraryService.Database.Converters;

public static class LibraryBooksConverter
{
    
    [return: NotNullIfNotNull("dbLibraryBooks")]
    public static CoreLibraryBooks? Convert(LibraryBooks? dbLibraryBooks)
    {
        if (dbLibraryBooks is null)
            return null;

        return new CoreLibraryBooks(dbLibraryBooks.Id, dbLibraryBooks.LibraryId, dbLibraryBooks.BooksId,
            dbLibraryBooks.Count);
    }
}