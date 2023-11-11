using System.Net;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using Rsoi.Lab3.GatewayService.Services.Confiugration;
using Rsoi.Lab3.GatewayService.Services.Exceptions;
using Rsoi.Lab3.ReservationService.Core;
using Rsoi.Lab3.ReservationService.Dto.Models;

namespace Rsoi.Lab3.GatewayService.Services.Clients;

public class ReservationServiceClient : IDisposable
{
    private readonly RestClient _restClient;
    public static readonly string ServiceName = "Reservation service"; 

    public ReservationServiceClient(IOptions<ServiceConfiguration> serviceConfiguration)
    {
        var options = new RestClientOptions(serviceConfiguration.Value.ReservationServiceAddress!);
        _restClient = new RestClient(options);        
    }
    
    public async Task<List<Reservation>?> GetReservationsForUserAsync(string username)
    {
        var request = new RestRequest($"user/{username}/reservations");

        try
        {
            var response = await _restClient.GetAsync(request);

            if (response.ResponseStatus is not ResponseStatus.Completed &&
                response.StatusCode is not HttpStatusCode.NotFound)
                throw new InternalServiceException(request, ServiceName, (int)response.StatusCode,
                    response.Content ?? "");

            return JsonConvert.DeserializeObject<List<Reservation>>(response.Content!);
        }
        catch (InternalServiceException e)
        {
            throw;
        }
        catch 
        {
            throw new InternalServiceException(request, ServiceName);
        }
        
    }

    public async Task<Reservation> CreateReservationAsync(string username, Guid booksId, Guid libraryId, DateTimeOffset tillDate)
    {
        var request = new RestRequest("reservations", Method.Post)
            .AddBody(new CreateReservationRequest(username, booksId, libraryId,
                DateOnly.FromDateTime(tillDate.UtcDateTime)));

        try
        {
            var response = await _restClient
                .PostAsync(request);

            if (response.ResponseStatus is not ResponseStatus.Completed &&
                response.StatusCode is not HttpStatusCode.NotFound)
                throw new InternalServiceException(request, ServiceName, (int)response.StatusCode,
                    response.Content ?? "");

            return JsonConvert.DeserializeObject<Reservation>(response.Content!)!;
        }
        catch (InternalServiceException e)
        {
            throw;
        }
        catch
        {
            throw new InternalServiceException(request, ServiceName);
        }
        
    }
    
    public async Task<CloseReservationResponse> CloseReservationAsync(Guid reservationId, DateTimeOffset closeDate)
    {
        var query = $"?closeDate={DateOnly.FromDateTime(closeDate.UtcDateTime)}";
        var request = new RestRequest($"reservations/{reservationId}/close" + query, Method.Post);

        try
        {
            var response = await _restClient
                .PostAsync(request);
        
            if (response.ResponseStatus is not ResponseStatus.Completed &&
                response.StatusCode is not HttpStatusCode.NotFound)
                throw new InternalServiceException(request, ServiceName, (int)response.StatusCode,
                    response.Content ?? "");

            return JsonConvert.DeserializeObject<CloseReservationResponse>(response.Content!)!;
        }
        catch (InternalServiceException e)
        {
            throw;
        }
        catch 
        {
            throw new InternalServiceException(request, ServiceName);
        }
    }

    public async Task DeleteReservationAsync(Guid reservationId)
    {
        var request = new RestRequest($"reservations/{reservationId}", Method.Delete);
        
        try
        {
            var response = await _restClient.DeleteAsync(request);
        
            if (response.ResponseStatus is not ResponseStatus.Completed &&
                response.StatusCode is not HttpStatusCode.NotFound)
                throw new InternalServiceException(request, ServiceName, (int)response.StatusCode,
                    response.Content ?? ""); 
        }
        catch (InternalServiceException e)
        {
            throw;
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