using AsyncFileTransferProcessor;
using AsyncFileTransferProcessor.Common;
using AsyncFileTransferProcessor.Common.Contracts;
using AsyncFileTransferProcessor.Contracts;
using AsyncFileTransferProcessor.Contracts.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace AsyncFileTransfer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var host = CreateHostBuilder(args).Build())
            {
                Task.Run(() => host.Start());

                var fileTransferListener = host.Services.GetRequiredService<IFileTransferListener>();
                fileTransferListener.Start();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IFileTransferListener, FileTransferListener>();
                services.AddSingleton<ILogger, FileTransferLogger>();
                services.AddSingleton<IFileSystemService, FileSystemService>();
                services.AddSingleton<IFileTransferService, FileTransferService>();
                services.AddSingleton<IFileTransferProcessorProvider, FileTransferProcessorProvider>();
                services.AddSingleton<ITaskQueue<FileTransferTask>, FileTransferQueue>();
                services.AddTransient<IAsyncFileTransferProcessor, FileTransferProcessor>();
                services.AddHostedService<FileTransferBackgroundConsumerService>();
            });
    }
}
