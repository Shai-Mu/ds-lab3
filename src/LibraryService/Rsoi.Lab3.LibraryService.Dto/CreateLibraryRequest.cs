using System.Runtime.Serialization;
using Rsoi.Lab3.LibraryService.Dto.Models;

namespace Rsoi.Lab3.LibraryService.Dto;

[DataContract]
public class CreateLibraryRequest
{
    [DataMember] 
    public List<Library> Libraries { get; set; }
    
    public CreateLibraryRequest(List<Library> libraries)
    {
        Libraries = libraries;
    }
}