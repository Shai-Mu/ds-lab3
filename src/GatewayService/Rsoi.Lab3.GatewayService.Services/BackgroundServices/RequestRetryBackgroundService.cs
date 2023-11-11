using Microsoft.Extensions.Hosting;
using RestSharp;

namespace Rsoi.Lab3.GatewayService.Services.BackgroundServices;

public class RequestRetryBackgroundService : BackgroundService
{
    private readonly UndoneRequestsQueue _undoneRequestsQueue;
    private readonly Dictionary<Uri, RestClient> _clients;

    public RequestRetryBackgroundService(UndoneRequestsQueue undoneRequestsQueue)
    {
        _undoneRequestsQueue = undoneRequestsQueue;
        _clients = new Dictionary<Uri, RestClient>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var undoneRequest = _undoneRequestsQueue.DequeRequest();
            
            
            if (undoneRequest is not null)
            {
                undoneRequest.Request.

                await undoneRequest;
            }
        }
    }
}