using AsyncFileTransferProcessor.Common.Contracts;
using AsyncFileTransferProcessor.Contracts;
using AsyncFileTransferProcessor.Contracts.Models;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncFileTransferProcessor
{
    public class FileTransferProcessor : IAsyncFileTransferProcessor
    {
        private readonly ILogger _logger;
        private readonly IFileSystemService _fileSystemService;
        private readonly ConcurrentQueue<FileTransferInfo> _filesQueue;       

        public FileTransferProcessor(ILogger logger, IFileSystemService fileSystemService)
        {
            _logger = logger;
            _fileSystemService = fileSystemService;
            _filesQueue = new ConcurrentQueue<FileTransferInfo>();
        }

        public void EnqueueFileTransferTask(FileTransferInfo fileTransferInfo)
        {
            _filesQueue.Enqueue(fileTransferInfo);
        }

        public Task StartProcessingAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => StartProcessing(cancellationToken));
        }

        private void StartProcessing(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    bool objectDequeued = _filesQueue.TryDequeue(out FileTransferInfo fileTransferInfo);
                    if (!objectDequeued)
                        continue;

                    ExecuteFileTransferTask(fileTransferInfo);
                }
                catch (Exception error)
                {
                    _logger.LogError(error.Message);
                }
            }
        }

        private void ExecuteFileTransferTask(FileTransferInfo fileTransferInfo)        {            

            var sourceFileName = _fileSystemService.GetFileName(fileTransferInfo.FilePath);
            var targetFilePath = _fileSystemService.ComposePath(fileTransferInfo.TargetFolderPath, sourceFileName);
            _fileSystemService.CopyFile(fileTransferInfo.FilePath, targetFilePath);
        }
    }
}
