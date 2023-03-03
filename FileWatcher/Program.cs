using FileWatcher;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Options;

//////Settings for Worker service Running
//IHost host = Host.CreateDefaultBuilder(args)
//	.ConfigureServices(services =>
//	{
//		services.AddHostedService<Worker>();
//		services.AddSingleton<IMyFileWatcher, MyFileWatcher>();
//		services.AddScoped<IFileConsumerService, FileConsumerService>();

//	})
//	.Build();

////Settings for WindowsService
IHost host = Host.CreateDefaultBuilder(args)
	.UseWindowsService()
	.ConfigureLogging(options => {
		if (OperatingSystem.IsWindows())
		{
			options.AddFilter<EventLogLoggerProvider>(level=>level >= LogLevel.Information);
		}

	})
	.ConfigureServices(services =>
	{
		services.AddHostedService<Worker>();
		services.AddSingleton<IMyFileWatcher, MyFileWatcher>();
		services.AddScoped<IFileConsumerService, FileConsumerService>();
		if (OperatingSystem.IsWindows())
		{
			services.Configure<EventLogSettings>(config =>
			{
				if (OperatingSystem.IsWindows())
				{
					config.LogName = "FolderWatcher Service";
					config.SourceName = "Folderwatcher Service Source";
				}
			});

		}

	})
	.Build();

await host.RunAsync();

