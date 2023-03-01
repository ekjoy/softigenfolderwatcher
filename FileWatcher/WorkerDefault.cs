namespace FileWatcher;

public class WorkerDefault : BackgroundService
{
    private readonly ILogger<WorkerDefault> _logger;

    public WorkerDefault(ILogger<WorkerDefault> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
