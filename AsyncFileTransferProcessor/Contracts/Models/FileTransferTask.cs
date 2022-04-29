namespace AsyncFileTransferProcessor.Contracts.Models
{
    public class FileTransferTask
    {
        public FileTransferTask(IAsyncFileTransferProcessor fileTransferProcessor, FileTransferInfo fileTransferInfo)
        {
            FileTransferProcessor = fileTransferProcessor;
            FileTransferInfo = fileTransferInfo;
        }

        public FileTransferInfo FileTransferInfo { get; }

        public IAsyncFileTransferProcessor FileTransferProcessor { get; }
    }
}
