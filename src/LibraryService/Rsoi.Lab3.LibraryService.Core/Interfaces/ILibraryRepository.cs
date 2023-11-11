﻿using Rsoi.Lab3.LibraryService.Core.Models;

namespace Rsoi.Lab3.LibraryService.Core.Interfaces;

public interface ILibraryRepository
{
    public Task<Guid> CreateLibraryAsync(Guid id, string name, string city, string address);

    public Task<List<Library>> GetLibrariesForCity(string city, int? page, int? siz);

    public Task<Library> GetLibraryAsync(Guid id);
}