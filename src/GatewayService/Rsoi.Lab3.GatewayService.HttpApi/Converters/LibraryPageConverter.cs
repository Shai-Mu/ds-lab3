using Rsoi.Lab3.LibraryService.Dto.Models;
using Rsoi.Lab3.GatewayService.HttpApi.Models;
using CoreLibrary = Rsoi.Lab3.LibraryService.Dto.Models.Library;

namespace Rsoi.Lab3.GatewayService.HttpApi.Converters;

public static class LibraryPageConverter
{
    public static LibraryPaginationResponse Convert(List<CoreLibrary>? libraries, decimal? page, decimal? size)
    {
        if (libraries is null)
            return new LibraryPaginationResponse(page, 
                size, 
                0,
                new List<LibraryResponse>());
        
        return new LibraryPaginationResponse(page, 
            size, 
            libraries.Count,
            libraries.ConvertAll(LibraryConverter.Convert)!);
    }
}