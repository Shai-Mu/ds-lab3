using RestSharp;

namespace Rsoi.Lab3.GatewayService.Services.BackgroundServices;

public class UndoneRequest
{
    public RestRequest Request { get; init; }
    
    public Uri ServiceAddress { get; init; }
    
    public DateTimeOffset LastTimeExecuted { get; private set; }

    public UndoneRequest(RestRequest request, Uri serviceAddress)
    {
        ServiceAddress = serviceAddress;
        Request = request;
        LastTimeExecuted = DateTimeOffset.Now;
    }

    public void UpdateLastTimeExecuted(DateTimeOffset executionTimestamp)
    {
        LastTimeExecuted = executionTimestamp;
    }
}