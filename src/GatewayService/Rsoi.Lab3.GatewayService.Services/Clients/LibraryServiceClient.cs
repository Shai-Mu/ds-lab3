using System.Net;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using Rsoi.Lab3.GatewayService.Services.Confiugration;
using Rsoi.Lab3.GatewayService.Services.Exceptions;
using Rsoi.Lab3.LibraryService.Dto.Models;

namespace Rsoi.Lab3.GatewayService.Services.Clients;

public class LibraryServiceClient : IDisposable
{
    private readonly RestClient _restClient;
    public static readonly string ServiceName = "Library service";

    public LibraryServiceClient(IOptions<ServiceConfiguration> serviceConfiguration)
    {
        var options = new RestClientOptions(serviceConfiguration.Value.LibraryServiceAddress!);
        _restClient = new RestClient(options);
    }

    public async Task<List<Library>?> GetCityLibrariesAsync(string city, int? page, int? size)
    {
        var query = $"?city={city}";

        if (page is not null && size is not null)
            query += $"&page={page}&size={size}";

        var request = new RestRequest($"libraries" + query);

        try
        {
            var response = await _restClient.GetAsync(request);

            if (response.ResponseStatus is not ResponseStatus.Completed &&
                response.StatusCode is not HttpStatusCode.NotFound)
                throw new InternalServiceException(request, ServiceName, (int)response.StatusCode,
                    response.Content ?? "");

            return JsonConvert.DeserializeObject<List<Library>>(response!.Content!);
        }
        catch
        {
            throw new InternalServiceException(request, ServiceName);
        }
        
    }

    public async Task<List<BooksWithCount>?> GetBooksForLibraryIdAsync(Guid libraryId, int? page, int? size, bool? showAll)
    {
        var query = string.Empty;

        if (page is not null && size is not null)
        {
            query += $"?page={page}&size={size}";
        }

        if (showAll is not null)
        {
            if (!query.Contains('?'))
                query += '?';
            else
                query += '&';

            query += "showAll={showAll}";
        }

        var request = new RestRequest($"libraries/{libraryId}/books" + query);

        try
        {
            var response = await _restClient.GetAsync(request);
        
            if (response.ResponseStatus is not ResponseStatus.Completed &&
                response.StatusCode is not HttpStatusCode.NotFound)
                throw new InternalServiceException(request, ServiceName, (int)response.StatusCode,
                    response.Content ?? "");
        
            return JsonConvert.DeserializeObject<List<BooksWithCount>>(response!.Content!);
        }
        catch
        {
            throw new InternalServiceException(request, ServiceName);
        }
    }

    public async Task<Books> GetBookAsync(Guid bookId)
    {
        var request = new RestRequest($"books/{bookId}");
        
        try
        {
            var response = await _restClient.GetAsync(request);
        
            if (response.ResponseStatus is not ResponseStatus.Completed &&
                response.StatusCode is not HttpStatusCode.NotFound)
                throw new InternalServiceException(request, ServiceName, (int)response.StatusCode,
                    response.Content ?? "");
        
            return JsonConvert.DeserializeObject<Books>(response!.Content!)!;
        }
        catch
        {
            throw new InternalServiceException(request, ServiceName);
        }
    }
    
    public async Task<Library> GetLibraryAsync(Guid libraryId)
    {
        var request = new RestRequest($"libraries/{libraryId}");

        try
        {
            var response = await _restClient.GetAsync(request);
        
            if (response.ResponseStatus is not ResponseStatus.Completed &&
                response.StatusCode is not HttpStatusCode.NotFound)
                throw new InternalServiceException(request, ServiceName, (int)response.StatusCode,
                    response.Content ?? "");
        
            return JsonConvert.DeserializeObject<Library>(response!.Content!)!;
        }
        catch
        {
            throw new InternalServiceException(request, ServiceName);
        }
       
    }

    public async Task TakeBookAsync(Guid bookId, Guid libraryId)
    {
        var request = new RestRequest($"libraries/{libraryId}/books/{bookId}/decrement", Method.Patch);

        try
        {
            var response = await _restClient.PatchAsync(request);
        
            if (response.ResponseStatus is not ResponseStatus.Completed &&
                response.StatusCode is not HttpStatusCode.NotFound)
                throw new InternalServiceException(request, ServiceName, (int)response.StatusCode,
                    response.Content ?? "");
        }
        catch
        {
            throw new InternalServiceException(request, ServiceName);
        }
        
    }

    public async Task ReturnBookAsync(Guid bookId, Guid libraryId, int newState)
    {
        var query = $"?newState={newState}";

        var request = new RestRequest($"libraries/{libraryId}/books/{bookId}/increment" + query, Method.Patch);
        request.Timeout = 10000;
            
        try
        {
            var response = await _restClient.PatchAsync(request);
        
            if (response.ResponseStatus is not ResponseStatus.Completed &&
                response.StatusCode is not HttpStatusCode.NotFound)
                throw new InternalServiceException(request, "Library service", (int)response.StatusCode,
                    response.Content ?? "");
        }
        catch
        {
            throw new InternalServiceException(request, ServiceName);
        }
        
    }

    public void Dispose()
    {
        _restClient.Dispose();
    }
    
}