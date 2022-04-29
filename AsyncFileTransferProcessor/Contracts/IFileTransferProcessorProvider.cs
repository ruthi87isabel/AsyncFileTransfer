namespace AsyncFileTransferProcessor.Contracts
{
    public interface IFileTransferProcessorProvider
    {
        IAsyncFileTransferProcessor GetFileTransferProcessor();
    }
}
