using AsyncFileTransferProcessor.Common.Contracts;
using AsyncFileTransferProcessor.Contracts.Models;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncFileTransferProcessor.Tests
{
    [TestFixture]
    public class FileTransferProcessorTest
    {
        private IFixture _fixture;
        private Mock<IFileSystemService> _fileSystemServiceMock;
        private CancellationTokenSource _cancellationTokenSource;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fileSystemServiceMock = _fixture.Freeze<Mock<IFileSystemService>>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        [TearDown]
        public void TearDown()
        {
            _cancellationTokenSource.Cancel();
        }
        

        [Test]
        public async Task StartProcessingAsync_WhenTasksAreEnqueued_FileTransferTasksAreExecuted()
        {
            // Arrange
            var count = _fixture.Create<int>();
            var subject = CreateFileTransferProcessorWithTasks(count);            

            // Act
            Task.Run(() => subject.StartProcessingAsync(_cancellationTokenSource.Token));

            // Assert
            await Task.Delay(2000);
            _fileSystemServiceMock.Verify(f => f.CopyFile(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
            _fileSystemServiceMock.Verify(f => f.CopyFile(It.IsAny<string>(), It.IsAny<string>()), Times.AtMost(count));
        }

        [Test]
        public async Task StartProcessingAsync_WhenNoTaskIsEnqueued_NoFileTransferTaskIsExecuted()
        {
            // Arrange
            var subject = CreateFileTransferProcessorWithTasks(0);

            // Act
            Task.Run(() => subject.StartProcessingAsync(_cancellationTokenSource.Token));

            // Assert
            await Task.Delay(1000);
            _fileSystemServiceMock.Verify(f => f.CopyFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        private FileTransferProcessor CreateFileTransferProcessorWithTasks(int count)
        {
            var subject = _fixture.Create<FileTransferProcessor>();

            for (int index = 0; index < count; index++)
            {
                var item = _fixture.Create<FileTransferInfo>();
                subject.EnqueueFileTransferTask(item);
            }

            return subject;
        }
    }
}
