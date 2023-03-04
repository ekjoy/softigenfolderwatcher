using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher
{
    public class MyFileWatcher : IMyFileWatcher
    {
        private string _directoryName1 = Path.Join(Environment.CurrentDirectory, "files");//change this to whatever you want
		 private Dictionary<int,string> _directories= new Dictionary<int,string>();
		private string _fileFilter = "*.*";
       
        ILogger<MyFileWatcher> _logger;
        IServiceProvider _serviceProvider;

        public MyFileWatcher(ILogger<MyFileWatcher> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            if(!Directory.Exists(_directoryName1))
                Directory.CreateDirectory(_directoryName1);
           
            _serviceProvider = serviceProvider;
			_directories.Add(0, "C:\\FileWatcher\\Input1");
			_directories.Add(1, "C:\\FileWatcher\\Input2");
		}

        public void Start()
        {
            
            for (int i = 0; i < 2; i++)
            {
                var _fileSystemWatcherDynamic = new FileSystemWatcher(_directories[i], _fileFilter);
				_fileSystemWatcherDynamic.NotifyFilter = NotifyFilters.Attributes
								 | NotifyFilters.CreationTime
								 | NotifyFilters.DirectoryName
								 | NotifyFilters.FileName
								 | NotifyFilters.LastAccess
								 | NotifyFilters.LastWrite
								 | NotifyFilters.Security
								 | NotifyFilters.Size;

				_fileSystemWatcherDynamic.Changed += _fileSystemWatcher_Changed;
				_fileSystemWatcherDynamic.Created += _fileSystemWatcher_Created;
				_fileSystemWatcherDynamic.Deleted += _fileSystemWatcher_Deleted;
				_fileSystemWatcherDynamic.Renamed += _fileSystemWatcher_Renamed;
				_fileSystemWatcherDynamic.Error += _fileSystemWatcher_Error;


				_fileSystemWatcherDynamic.EnableRaisingEvents = true;
				_fileSystemWatcherDynamic.IncludeSubdirectories = false;

				_logger.LogInformation($"File Watching has started for directory {_directories[i]}");
			}

        }

        private void _fileSystemWatcher_Error(object sender, ErrorEventArgs e)
        {
            _logger.LogInformation($"File error event {e.GetException().Message}");
        }

        private void _fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            _logger.LogInformation($"File rename event for file {e.FullPath}");
        }

        private void _fileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation($"File deleted event for file {e.FullPath}");
        }

        private void _fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {

        }

        private void _fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var consumerService = scope.ServiceProvider.GetRequiredService<IFileConsumerService>();
                Task.Run(() => consumerService.ConsumeFile(e.FullPath));
            }
        }
    }
}
