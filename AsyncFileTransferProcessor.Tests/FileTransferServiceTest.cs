using AsyncFileTransferProcessor.Common.Contracts;
using AsyncFileTransferProcessor.Contracts;
using AsyncFileTransferProcessor.Contracts.Models;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AsyncFileTransferProcessor.Tests
{
    [TestFixture]
    public class FileTransferServiceTest
    {
        private IFixture _fixture;        
        private FileTransferService _subject;
        private Mock<IFileSystemService> _fileSystemServiceMock;
        private Mock<IFileTransferProcessorProvider> _fileTransferProcessorProviderMock;
        private Mock<ITaskQueue<FileTransferTask>> _taskQueueMock;

        private string _sourceFolder;
        private string _destinationFolder;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());            

            _fileSystemServiceMock = _fixture.Freeze<Mock<IFileSystemService>>();
            _fileTransferProcessorProviderMock = _fixture.Freeze<Mock<IFileTransferProcessorProvider>>();
            _taskQueueMock = _fixture.Freeze<Mock<ITaskQueue<FileTransferTask>>>();

            _sourceFolder = _fixture.Create<string>();
            _destinationFolder = _fixture.Create<string>();
            _cancellationToken = _fixture.Create<CancellationToken>();

            _subject = _fixture.Create<FileTransferService>();
        }

        [Test]
        public void TransferFiles_WhenSourceFolderDoesNotExist_ShouldReturnNotSuccessfulResult()
        {
            // Arrange            
            _fileSystemServiceMock.Setup(x => x.DirectoryExists(_sourceFolder)).Returns(false);

            // Act
            var result = _subject.TransferFiles(_sourceFolder, _destinationFolder, _cancellationToken);

            // Assert
            Assert.IsFalse(result.Successful);
        }

        [Test]
        public void TransferFiles_WhenDestinationFolderDoesNotExist_ShouldReturnNotSuccessfulResult()
        {
            // Arrange
            _fileSystemServiceMock.Setup(x => x.DirectoryExists(_destinationFolder)).Returns(false);

            // Act
            var result = _subject.TransferFiles(_sourceFolder, _destinationFolder, _cancellationToken);

            // Assert
            Assert.IsFalse(result.Successful);
        }

        [Test]
        public void TransferFiles_WhenSourceAndDestinationFoldersDoExist_ShouldReturnSuccessfulResult()
        {
            // Arrange
            _fileSystemServiceMock.Setup(x => x.DirectoryExists(It.IsAny<string>())).Returns(true);

            // Act
            var result = _subject.TransferFiles(_sourceFolder, _destinationFolder, _cancellationToken);

            // Assert
            Assert.IsTrue(result.Successful);
        }

        [Test]
        public void TransferFiles_WhenExceptionIsThrown_ShouldReturnNotSuccessfulResultAndException()
        {
            // Arrange
            var exception = _fixture.Create<Exception>();

            _fileSystemServiceMock.Setup(x => x.DirectoryExists(It.IsAny<string>())).Returns(true);
            _fileSystemServiceMock.Setup(x => x.DirectoryFilesList(It.IsAny<string>())).Throws(exception);

            // Act
            var result = _subject.TransferFiles(_sourceFolder, _destinationFolder, _cancellationToken);

            // Assert
            Assert.IsFalse(result.Successful);
            Assert.IsNotNull(result.ExceptionThrown);
            Assert.AreEqual(exception, result.ExceptionThrown);
        }

        [Test]
        public void TransferFiles_WhenSourceFoldersHasNoFiles_ShouldNotEnqueueFileTransferTasks()
        {
            // Arrange
            var filesList = new string[0];
            var fileExt = _fixture.Create<string>();

            _fileSystemServiceMock.Setup(x => x.DirectoryExists(It.IsAny<string>())).Returns(true);
            _fileSystemServiceMock.Setup(x => x.DirectoryFilesList(It.IsAny<string>())).Returns(filesList);
            _fileSystemServiceMock.Setup(x => x.GetFileExtension(It.IsAny<string>())).Returns(fileExt);

            // Act
            var result = _subject.TransferFiles(_sourceFolder, _destinationFolder, _cancellationToken);

            // Assert
            _taskQueueMock.Verify(q => q.EnqueueItem(It.IsAny<FileTransferTask>()), Times.Never);
            Assert.IsTrue(result.Successful);
            Assert.IsNull(result.ExceptionThrown);
        }

        [Test]
        public void TransferFiles_WhenSourceFoldersHasFiles_ShouldEnqueueFileTransferTasks()
        {
            // Arrange
            var filesList = _fixture.CreateMany<string>(_fixture.Create<int>()).ToArray();
            var fileExt = _fixture.Create<string>();

            _fileSystemServiceMock.Setup(x => x.DirectoryExists(It.IsAny<string>())).Returns(true);
            _fileSystemServiceMock.Setup(x => x.DirectoryFilesList(It.IsAny<string>())).Returns(filesList);
            _fileSystemServiceMock.Setup(x => x.GetFileExtension(It.IsAny<string>())).Returns(fileExt);

            // Act
            var result = _subject.TransferFiles(_sourceFolder, _destinationFolder, _cancellationToken);

            // Assert
            _taskQueueMock.Verify(q => q.EnqueueItem(It.IsAny<FileTransferTask>()), Times.Exactly(filesList.Length));
            Assert.IsTrue(result.Successful);
            Assert.IsNull(result.ExceptionThrown);
        }

        [Test]
        public void TransferFiles_WhenSourceFoldersHasFiles_ShouldCreateFileTransferProcessorsForeachExtension()
        {
            // Arrange
            var filesList = _fixture.CreateMany<string>(_fixture.Create<int>()).ToArray();
            var extensionsList = new List<string>();

            _fileSystemServiceMock.Setup(x => x.DirectoryExists(It.IsAny<string>())).Returns(true);
            _fileSystemServiceMock.Setup(x => x.DirectoryFilesList(It.IsAny<string>())).Returns(filesList);
            
            foreach(var item in filesList)
            {
                var fileExt = _fixture.Create<string>();
                extensionsList.Add(fileExt);
                _fileSystemServiceMock.Setup(x => x.GetFileExtension(item)).Returns(fileExt);                
            }            

            // Act
            var result = _subject.TransferFiles(_sourceFolder, _destinationFolder, _cancellationToken);

            // Assert
            var differentExtensionsList = extensionsList.Distinct().ToArray();
            _fileTransferProcessorProviderMock.Verify(p => p.GetFileTransferProcessor(), Times.Exactly(differentExtensionsList.Length));            
            Assert.IsTrue(result.Successful);
            Assert.IsNull(result.ExceptionThrown);
        }
    }
}