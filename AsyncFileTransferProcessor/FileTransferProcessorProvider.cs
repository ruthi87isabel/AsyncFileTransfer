using AsyncFileTransferProcessor.Contracts;
using System;

namespace AsyncFileTransferProcessor
{
    public class FileTransferProcessorProvider : IFileTransferProcessorProvider
    {
        private readonly IServiceProvider _serviceProvider;
        public FileTransferProcessorProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IAsyncFileTransferProcessor GetFileTransferProcessor()
            => (IAsyncFileTransferProcessor)_serviceProvider.GetService(typeof(IAsyncFileTransferProcessor));
    }
}
