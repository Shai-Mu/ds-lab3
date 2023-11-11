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
    public readonly Uri ServiceAddress;
    private readonly ServiceState _serviceState;

    public RatingServiceClient(IOptions<ServiceConfiguration> serviceConfiguration)
    {
        _serviceState = new ServiceState();
        ServiceAddress = serviceConfiguration.Value.RatingServiceAddress!;
        var options = new RestClientOptions(ServiceAddress);
        _restClient = new RestClient(options);
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
        catch (InternalServiceException e)
        {
            throw;
        }
        catch
        {
            _serviceState.RecordServiceUnable();

            throw new InternalServiceException(request, _serviceName);
        }
        
    }

    public async Task ModifyRatingForUserAsync(string username, int ratingValue)
    {
        var query = $"?stars={ratingValue}&username={username}";
        var request = new RestRequest($"ratings/modify" + query, Method.Patch);

        try
        {
            var response = await _restClient.PatchAsync(request);

            if (response.ResponseStatus is not ResponseStatus.Completed &&
                response.StatusCode is not HttpStatusCode.NotFound)
                throw new InternalServiceException(request, _serviceName, (int)response.StatusCode,
                    response.Content ?? "");
        }
        catch (InternalServiceException e)
        {
            throw;
        }
        catch
        {
            _serviceState.RecordServiceUnable();

            throw new InternalServiceException(request, _serviceName);
        }
       
    }

    public void Dispose()
    {
        _restClient.Dispose();
    }
}