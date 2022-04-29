using AsyncFileTransferProcessor.Common.Contracts;
using AsyncFileTransferProcessor.Contracts;
using AsyncFileTransferProcessor.Contracts.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncFileTransferProcessor
{
    public class FileTransferBackgroundConsumerService : BackgroundService
    {
        private readonly ITaskQueue<FileTransferTask> _taskQueue;
        private readonly ILogger _logger;

        public FileTransferBackgroundConsumerService(ITaskQueue<FileTransferTask> taskQueue, ILogger logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() => StartProcessingFileTransferTasks(cancellationToken));
        }

        private void StartProcessingFileTransferTasks(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var fileTransferTask = _taskQueue.DequeueItem();
                    if (fileTransferTask == null)
                        continue;

                    ProcessFileTransferTask(fileTransferTask);
                }
                catch (Exception error)
                {
                    _logger.LogError(error.Message);
                }
            }
        }

        private void ProcessFileTransferTask(FileTransferTask fileTransferTask)
        {
            var fileTransferProcessor = fileTransferTask.FileTransferProcessor;
            fileTransferProcessor.EnqueueFileTransferTask(fileTransferTask.FileTransferInfo);
        }
    }
}
