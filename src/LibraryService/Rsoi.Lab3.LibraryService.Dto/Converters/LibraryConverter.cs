using Rsoi.Lab3.LibraryService.Dto.Models;
using CoreLibrary = Rsoi.Lab3.LibraryService.Core.Models.Library;
using DtoLibrary = Rsoi.Lab3.LibraryService.Dto.Models.Library;

namespace Rsoi.Lab3.LibraryService.Dto.Converters;

public static class LibraryConverter
{
    public static Library Convert(CoreLibrary coreLibrary)
    {
        return new Library(coreLibrary.Id, coreLibrary.Name, coreLibrary.City, coreLibrary.Address);
    }
}