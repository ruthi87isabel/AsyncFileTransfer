namespace AsyncFileTransferProcessor.Common.Contracts
{
    public interface ILogger
    {
        void LogInfo(string log);
        void LogError(string log);
    }
}
