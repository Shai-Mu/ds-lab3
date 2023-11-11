using System.Net;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using Rsoi.Lab3.GatewayService.Services.Confiugration;
using Rsoi.Lab3.GatewayService.Services.Exceptions;
using Rsoi.Lab3.RatingService.Dto.Models;

namespace Rsoi.Lab3.GatewayService.Services.Clients;

public class RatingServiceClient : IDisposable
{
    private readonly RestClient _restClient;
    private static readonly string _serviceName = "Rating service";
    public readonly Uri _serviceAddress;

    public RatingServiceClient(IOptions<ServiceConfiguration> serviceConfiguration)
    {
        var options = new RestClientOptions(serviceConfiguration.Value.RatingServiceAddress!);
        _restClient = new RestClient(options);
        _serviceAddress = serviceConfiguration.Value.RatingServiceAddress!;
    }
    
    public async Task<RatingResponse?> GetRatingForUserAsync(string username)
    {
        var query = $"?username={username}";
        var request = new RestRequest($"ratings" + query);

        try
        {
            var response = await _restClient.GetAsync(request);

            if (response.ResponseStatus is not ResponseStatus.Completed &&
                response.StatusCode is not HttpStatusCode.NotFound)
                throw new InternalServiceException(request, _serviceName, (int)response.StatusCode,
                    response.Content ?? "");

            return JsonConvert.DeserializeObject<RatingResponse>(response!.Content!);
        }
        catch
        {
            throw new InternalServiceException(request, _serviceName);
        }
        
    }

    public async Task EditRatingForUserAsync(Guid ratingId, int ratingValue)
    {
        var query = $"?stars={ratingValue}";
        var request = new RestRequest($"ratings/{ratingId}" + query, Method.Patch);

        try
        {
            var response = await _restClient.PatchAsync(request);

            if (response.ResponseStatus is not ResponseStatus.Completed &&
                response.StatusCode is not HttpStatusCode.NotFound)
                throw new InternalServiceException(request, _serviceName, (int)response.StatusCode,
                    response.Content ?? "");
        }
        catch
        {
            throw new InternalServiceException(request, _serviceName);
        }
       
    }

    public void Dispose()
    {
        _restClient.Dispose();
    }
}