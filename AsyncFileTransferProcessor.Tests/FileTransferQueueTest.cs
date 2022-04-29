using AsyncFileTransferProcessor.Contracts.Models;
using AutoFixture;
using AutoFixture.AutoMoq;
using NUnit.Framework;
using System.Linq;

namespace AsyncFileTransferProcessor.Tests
{
    [TestFixture]
    public class FileTransferQueueTest
    {
        private IFixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());            
        }

        [Test]
        public void EnqueueItem_WhenCalledWithNullParam_NoItemIsInserted()
        {
            // Arrange
            var count = _fixture.Create<int>();
            var subject = CreateFileTransferQueueWithItems(count);
            
            // Act
            subject.EnqueueItem(null);

            // Assert
            var countAfterItemInserted = subject.Count();
            Assert.AreEqual(count, countAfterItemInserted);
            Assert.IsFalse(subject.Contains(null));
        }

        [Test]
        public void EnqueueItem_WhenCalledWithNotNullParam_ItemIsInserted()
        {
            // Arrange            
            var count = _fixture.Create<int>();
            var subject = CreateFileTransferQueueWithItems(count);
            var item = _fixture.Create<FileTransferTask>();

            // Act
            subject.EnqueueItem(item);

            // Assert
            var countAfterItemInserted = subject.Count();
            Assert.AreEqual(count + 1, countAfterItemInserted);
            Assert.IsTrue(subject.Contains(item));
        }

        [Test]
        public void DequeueItem_WhenQueueIsEmpty_ReturnsNullItem()
        {
            // Arrange
            var subject = CreateFileTransferQueueWithItems(0);

            // Act
            var item = subject.DequeueItem();

            // Assert
            Assert.IsNull(item);
        }

        [Test]
        public void DequeueItem_WhenQueueIsNotEmpty_ReturnsNotNullItem()
        {
            // Arrange
            var count = _fixture.Create<int>();
            var subject = CreateFileTransferQueueWithItems(count);

            // Act
            var item = subject.DequeueItem();

            // Assert
            var countAfterItemInserted = subject.Count();
            Assert.AreEqual(count - 1, countAfterItemInserted);
            Assert.IsNotNull(item);            
            Assert.IsFalse(subject.Contains(item));
        }

        private FileTransferQueue CreateFileTransferQueueWithItems(int count)
        {
            var subject = _fixture.Create<FileTransferQueue>();

            for(int index = 0; index < count; index++)
            {
                var item = _fixture.Create<FileTransferTask>();
                subject.EnqueueItem(item);
            }

            return subject;
        }
    }
}
