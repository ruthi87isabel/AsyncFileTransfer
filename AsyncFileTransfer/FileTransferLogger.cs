using AsyncFileTransferProcessor.Common.Contracts;
using System;

namespace AsyncFileTransfer
{
    public class FileTransferLogger : ILogger
    {
        public void LogInfo(string log)
        {            
            Console.WriteLine(log);
        }

        public void LogError(string log)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(log);
            Console.ResetColor();
        }
    }
}
