using AsyncFileTransferProcessor.Contracts.Models;
using System.Threading;

namespace AsyncFileTransferProcessor.Contracts
{
    public interface IFileTransferService
    {
        FileTransferResult TransferFiles(string sourceFolder, string destinationFolder, CancellationToken cancellationToken);
    }
}
