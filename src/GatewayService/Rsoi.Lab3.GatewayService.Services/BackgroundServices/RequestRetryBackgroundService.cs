using Microsoft.Extensions.Hosting;
using RestSharp;

namespace Rsoi.Lab3.GatewayService.Services.BackgroundServices;

public class RequestRetryBackgroundService : BackgroundService
{
    private readonly UndoneRequestsQueue _undoneRequestsQueue;
    private readonly Dictionary<string, RestClient> _clients;

    public RequestRetryBackgroundService(UndoneRequestsQueue undoneRequestsQueue)
    {
        _undoneRequestsQueue = undoneRequestsQueue;
        _clients = new Dictionary<string, RestClient>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            DateTimeOffset iterationTimestamp = DateTimeOffset.Now;

            var executingTasks = new List<(string, Task<RestResponse>)>();
            
            foreach (var uriString in _undoneRequestsQueue.Uris)
            {
                if (!_clients.ContainsKey(uriString))
                    CreateClient(uriString);

                var client = _clients[uriString];

                if (!_undoneRequestsQueue.IsEmpty(uriString))
                {
                    var undoneRequest = _undoneRequestsQueue.DequeRequest(uriString);

                    executingTasks.Add((uriString, client.ExecuteAsync(undoneRequest!.Request, stoppingToken)));
                }
            }

            await Task.WhenAll(executingTasks.Select(u => u.Item2));

            var uncompletedTasks = executingTasks.Where(r => 
                r.Item2.Result.ErrorException is not null);

            foreach (var uncompletedTask in uncompletedTasks)
            {
                _undoneRequestsQueue.ReturnToQueueLastRequest(uncompletedTask.Item1);
            }

            TimeSpan timeElapsed = DateTimeOffset.Now - iterationTimestamp;

            if (timeElapsed <= TimeSpan.FromSeconds(5))
                await Task.Delay(TimeSpan.FromSeconds(5) - timeElapsed, stoppingToken);

        }
    }

    private void CreateClient(string uri)
    {
        var uriObject = new Uri(uri);

        var options = new RestClientOptions(uriObject)
        {
            MaxTimeout = 10000
        };

        _clients.Add(uri, new RestClient(options));
    }
    
}