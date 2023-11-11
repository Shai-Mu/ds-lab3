namespace Rsoi.Lab3.GatewayService.Services.Clients;

public class ServiceState
{
    private int _serviceUnableCounter = 0;
    private bool _fallbackRequired = false;

    public int ServiceUnableCounter => _serviceUnableCounter;

    public event EventHandler<EventArgs> ServiceCheckoutRequired;

    public Timer? _serviceCheckoutTimer;

    public int MaxUnableAmount { get; init; }

    public bool FallbackRequired => _fallbackRequired;

    public ServiceState(int maxUnableAmount = 5)
    {
        MaxUnableAmount = maxUnableAmount;
    }

    public void RecordServiceUnable()
    {
        Interlocked.Increment(ref _serviceUnableCounter);

        if (_serviceUnableCounter >= MaxUnableAmount)
        {
            if (!_fallbackRequired)
            {
                _serviceCheckoutTimer = new Timer(state => OnServiceCheckoutRequired(), null, TimeSpan.FromSeconds(5), Timeout.InfiniteTimeSpan);
            }

            _fallbackRequired = true;

        }
    }

    public void RecordServiceAccessible()
    {
        Interlocked.Add(ref _serviceUnableCounter, -_serviceUnableCounter);
        _fallbackRequired = false;
    }


    protected virtual void OnServiceCheckoutRequired()
    {
        ServiceCheckoutRequired?.Invoke(this, EventArgs.Empty);
        _serviceCheckoutTimer?.Dispose();
        _serviceCheckoutTimer = null;
    }
}