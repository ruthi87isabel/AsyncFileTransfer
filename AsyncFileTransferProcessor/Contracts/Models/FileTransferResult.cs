using System;

namespace AsyncFileTransferProcessor.Contracts.Models
{
    public class FileTransferResult
    {
        public bool Successful { get; set; }
        public Exception ExceptionThrown { get; set; }
    }
}
