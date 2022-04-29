using AsyncFileTransferProcessor.Contracts.Models;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncFileTransferProcessor.Contracts
{
    public interface IAsyncFileTransferProcessor
    {
        Task StartProcessingAsync(CancellationToken cancellationToken);

        void EnqueueFileTransferTask(FileTransferInfo fileTransferInfo);        
    }
}
