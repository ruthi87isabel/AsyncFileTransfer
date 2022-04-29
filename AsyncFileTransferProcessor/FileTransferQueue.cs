using AsyncFileTransferProcessor.Contracts;
using AsyncFileTransferProcessor.Contracts.Models;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AsyncFileTransferProcessor
{
    public class FileTransferQueue : ITaskQueue<FileTransferTask>
    {
        private readonly ConcurrentQueue<FileTransferTask> _filesQueue;

        public FileTransferQueue()
        {
            _filesQueue = new ConcurrentQueue<FileTransferTask>();
        }

        public void EnqueueItem(FileTransferTask item)
        {
            if (item == null) 
                return;

            _filesQueue.Enqueue(item);
        }

        public FileTransferTask DequeueItem()
        {
            var success = _filesQueue.TryDequeue(out FileTransferTask item);
            return success ? item : null;
        }

        public IEnumerator<FileTransferTask> GetEnumerator() => _filesQueue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _filesQueue.GetEnumerator();
    }
}
