using AsyncFileTransferProcessor.Common.Contracts;
using AsyncFileTransferProcessor.Contracts;
using AsyncFileTransferProcessor.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AsyncFileTransferProcessor
{
    public class FileTransferService : IFileTransferService
    {
        private readonly ILogger _logger;
        private readonly IFileSystemService _fileSystemService;
        private readonly ITaskQueue<FileTransferTask> _fileTransferTasksQueue;
        private readonly IFileTransferProcessorProvider _fileTransferProcessorProvider;        
        private IDictionary<string, IAsyncFileTransferProcessor> _fileTransferProcessors;

        public FileTransferService(
            ILogger logger,
            IFileSystemService fileSystemService,
            ITaskQueue<FileTransferTask> fileTransferTasksQueue,
            IFileTransferProcessorProvider fileTransferProcessorProvider)
        {
            _logger = logger;
            _fileSystemService = fileSystemService;
            _fileTransferTasksQueue = fileTransferTasksQueue;
            _fileTransferProcessorProvider = fileTransferProcessorProvider;            
            _fileTransferProcessors = new Dictionary<string, IAsyncFileTransferProcessor>();
        }

        public FileTransferResult TransferFiles(string sourceFolder, string destinationFolder, CancellationToken cancellationToken)
        {           
            bool transferSuccessful = false;
            Exception errorThrown = null;

            try
            {
                bool sourceFolderExist = _fileSystemService.DirectoryExists(sourceFolder);
                bool destinationFolderExist = _fileSystemService.DirectoryExists(destinationFolder);

                if (sourceFolderExist && destinationFolderExist)
                {
                    string[] files = _fileSystemService.DirectoryFilesList(sourceFolder);
                    foreach (var filePath in files)
                    {
                        var fileTransferInfo = BuildFileTransferInfo(filePath, destinationFolder);
                        EnqueueFileTransferTask(fileTransferInfo, cancellationToken);
                    }
                    transferSuccessful = true;
                }
            }
            catch (Exception error)
            {
                transferSuccessful = false;
                errorThrown = error;
                _logger.LogError(error.Message);
            }        

            return new FileTransferResult
            {
                Successful = transferSuccessful,
                ExceptionThrown = errorThrown
            };
        }

        private FileTransferInfo BuildFileTransferInfo(string filePath, string destinationFolderPath)
        {
            return new FileTransferInfo
            {
                FilePath = filePath,
                FileExtension = _fileSystemService.GetFileExtension(filePath),
                TargetFolderPath = destinationFolderPath
            };
        }

        private void EnqueueFileTransferTask(FileTransferInfo fileTransferInfo, CancellationToken cancellationToken)
        {
            var fileExtension = fileTransferInfo.FileExtension.ToLower();

            if (!_fileTransferProcessors.ContainsKey(fileExtension))
            {
                var fileTransferProcessorForCurrentExtension = _fileTransferProcessorProvider.GetFileTransferProcessor();
                fileTransferProcessorForCurrentExtension.StartProcessingAsync(cancellationToken);
                _fileTransferProcessors.Add(fileExtension, fileTransferProcessorForCurrentExtension);
            }

            var fileTransferTask = new FileTransferTask(_fileTransferProcessors[fileExtension], fileTransferInfo);
            _fileTransferTasksQueue.EnqueueItem(fileTransferTask);
        }
    }
}
