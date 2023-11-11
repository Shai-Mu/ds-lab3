using System.Diagnostics.CodeAnalysis;
using Rsoi.Lab3.LibraryService.Database.Converters.EnumConverters;
using Rsoi.Lab3.LibraryService.Database.Models;
using DbLibrary = Rsoi.Lab3.LibraryService.Database.Models.Library;
using CoreLibrary = Rsoi.Lab3.LibraryService.Core.Models.Library;

namespace Rsoi.Lab3.LibraryService.Database.Converters;

public class LibraryConverter
{
    [return: NotNullIfNotNull("dbLibrary")]
    public static CoreLibrary? Convert(Library? dbLibrary)
    {
        if (dbLibrary is null)
            return null;

        return new CoreLibrary(dbLibrary.Id,
            dbLibrary.Name,
            dbLibrary.City,
            dbLibrary.Address);
    }
}