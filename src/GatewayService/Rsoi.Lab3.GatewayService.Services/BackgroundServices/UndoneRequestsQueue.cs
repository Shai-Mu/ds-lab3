using System.Collections.Concurrent;

namespace Rsoi.Lab3.GatewayService.Services.BackgroundServices;

public class UndoneRequestsQueue
{
    private readonly ConcurrentQueue<UndoneRequest> _queuedTasks;

    public UndoneRequestsQueue()
    {
        _queuedTasks = new ConcurrentQueue<UndoneRequest>();
    }

    public void AddTaskToQueue(UndoneRequest undoneRequest)
    {
        _queuedTasks.Enqueue(undoneRequest);
    }

    public UndoneRequest? DequeRequest()
    {
        _queuedTasks.TryDequeue(out var undoneRequest);

        return undoneRequest;
    }
}