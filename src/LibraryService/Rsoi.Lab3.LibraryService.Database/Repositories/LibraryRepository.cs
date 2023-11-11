﻿using Microsoft.EntityFrameworkCore;
using Rsoi.Lab3.LibraryService.Core.Interfaces;
using Rsoi.Lab3.LibraryService.Database.Models;
using Rsoi.Lab3.LibraryService.Database.Converters;
using Library = Rsoi.Lab3.LibraryService.Core.Models.Library;

namespace Rsoi.Lab3.LibraryService.Database.Repositories;

public class LibraryRepository : ILibraryRepository
{
    private readonly LibraryContext _libraryContext;

    public LibraryRepository(LibraryContext libraryContext)
    {
        _libraryContext = libraryContext;
    }

    public async Task<Guid> CreateLibraryAsync(Guid id, string name, string city, string address)
    {
        var library = new Models.Library(id,
            name, 
            city,
            address);

        await _libraryContext.Libraries.AddAsync(library);

        await _libraryContext.SaveChangesAsync();
        
        return library.Id;
    }

    public async Task<List<Library>> GetLibrariesForCity(string city, int? page, int? size)
    {
        var getLibrariesQuery = _libraryContext.Libraries
            .AsNoTracking()
            .Where(l => l.City == city);

        if (page is not null && size is not null)
            getLibrariesQuery = getLibrariesQuery
                .Skip((page.Value - 1) * size.Value);
                
        if (size is not null)
            getLibrariesQuery = getLibrariesQuery
                .Take(size.Value);

        var libraries = await getLibrariesQuery.ToListAsync();

        return libraries.Select(LibraryConverter.Convert).ToList()!;
    }

    public async Task<Library> GetLibraryAsync(Guid id)
    {
        var libraries = await _libraryContext.Libraries
            .AsNoTracking()
            .FirstAsync(l => l.Id == id);

        return LibraryConverter.Convert(libraries);
    }
}