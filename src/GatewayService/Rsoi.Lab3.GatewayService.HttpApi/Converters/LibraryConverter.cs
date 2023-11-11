using System.Diagnostics.CodeAnalysis;
using Rsoi.Lab3.GatewayService.HttpApi.Models;
using Library = Rsoi.Lab3.LibraryService.Dto.Models.Library;

namespace Rsoi.Lab3.GatewayService.HttpApi.Converters;

public static class LibraryConverter
{
    [return: NotNullIfNotNull("libraryServiceLibrary")]
    public static LibraryResponse? Convert(Library? libraryServiceLibrary)
    {
        if (libraryServiceLibrary is null)
            return null;
        
        return new LibraryResponse(libraryServiceLibrary.Id, 
            libraryServiceLibrary.Name, 
            libraryServiceLibrary.Address,
            libraryServiceLibrary.City);
    }
}