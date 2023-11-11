using System.Collections.Concurrent;

namespace Rsoi.Lab3.GatewayService.Services.BackgroundServices;

public class UndoneRequestsQueue
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<UndoneRequest>> _queuedTasksPerUri;
    private readonly ConcurrentDictionary<string, UndoneRequest> _lastRequestsTakenPerUri;

    public UndoneRequestsQueue()
    {
        _queuedTasksPerUri = new ConcurrentDictionary<string, ConcurrentQueue<UndoneRequest>>();
        _lastRequestsTakenPerUri = new ConcurrentDictionary<string, UndoneRequest>();
    }

    public void AddTaskToQueue(UndoneRequest undoneRequest)
    {
        var queue = _queuedTasksPerUri.GetOrAdd(undoneRequest.ServiceAddress.ToString(), new ConcurrentQueue<UndoneRequest>());
        queue.Enqueue(undoneRequest);
    }

    public UndoneRequest? DequeRequest(string uri)
    {
        if (!_queuedTasksPerUri.ContainsKey(uri))
            return null;
        
        _queuedTasksPerUri[uri].TryDequeue(out var undoneRequest);
        if (undoneRequest is not null)
            _lastRequestsTakenPerUri[uri] = undoneRequest;

        return undoneRequest;
    }

    public void ReturnToQueueLastRequest(string uri)
    {
        AddTaskToQueue(_lastRequestsTakenPerUri[uri]);
    }

    public bool IsEmpty(string uri)
    {
        return !_queuedTasksPerUri.ContainsKey(uri) || _queuedTasksPerUri[uri].IsEmpty;
    }

    public ICollection<string> Uris => _queuedTasksPerUri.Keys;
}