using AsyncFileTransferProcessor.Contracts;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using NUnit.Framework;
using System;

namespace AsyncFileTransferProcessor.Tests
{
    [TestFixture]
    public class FileTransferProcessorProviderTest
    {
        private IFixture _fixture;
        private Mock<IServiceProvider> _serviceProviderMock;
        private FileTransferProcessorProvider _subject;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            var asyncFileTransferProcessor = _fixture.Create<IAsyncFileTransferProcessor>();
            _serviceProviderMock = _fixture.Freeze<Mock<IServiceProvider>>();
            _serviceProviderMock.Setup(s => s.GetService(typeof(IAsyncFileTransferProcessor))).Returns(asyncFileTransferProcessor);
            
            _subject = _fixture.Create<FileTransferProcessorProvider>();
        }

        [Test]
        public void GetFileTransferProcessor_WhenCalled_ReturnsIAsyncFileTransferProcessorInstance()
        {
            // Act
            var result = _subject.GetFileTransferProcessor();

            // Assert
            Assert.IsInstanceOf<IAsyncFileTransferProcessor>(result);
        }
    }
}
