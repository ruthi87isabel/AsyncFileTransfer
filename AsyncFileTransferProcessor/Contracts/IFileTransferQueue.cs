using System.Collections.Generic;

namespace AsyncFileTransferProcessor.Contracts
{
    public interface ITaskQueue<T> : IEnumerable<T>
    {
        void EnqueueItem(T item);

        T DequeueItem();
    }
}
