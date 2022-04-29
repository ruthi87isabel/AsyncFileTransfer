using AsyncFileTransfer.Command;
using AsyncFileTransferProcessor.Common.Contracts;
using AsyncFileTransferProcessor.Contracts;
using CommandLine;
using System;
using System.Linq;
using System.Threading;

namespace AsyncFileTransfer
{
    public interface IFileTransferListener
    {
        void Start();
        void Stop();
    }

    public class FileTransferListener : IFileTransferListener
    {        
        private readonly IFileTransferService _fileTransferService;
        private readonly IFileSystemService _fileSystemService;
        private readonly ILogger _logger;
        private readonly Parser _commandLineParser;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public FileTransferListener(
            IFileTransferService fileTransferService,
            IFileSystemService fileSystemService,             
            ILogger logger)
        {
            _fileSystemService = fileSystemService;
            _fileTransferService = fileTransferService;
            _logger = logger;
            _commandLineParser = new Parser(s => SetParserSettings(s));
            _cancellationTokenSource = new CancellationTokenSource();            
        }

        public void Start()
        {            
            var cancellationToken = _cancellationTokenSource.Token;            

            while (!cancellationToken.IsCancellationRequested)
            {
                WriteInstructionLines();

                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                    continue;

                var commandAction = ExtractActionFromCommand(input);
                switch (commandAction)
                {
                    case FileTransferCommand.Transfer:
                        var (sourceFolder, destinationFolder) = ExtractSourceAndDestinationFoldersFromCommand(input);
                        if (ValidFolderPaths(sourceFolder, destinationFolder))
                        {
                            var fileTransferResult = _fileTransferService.TransferFiles(sourceFolder, destinationFolder, cancellationToken);
                            if (fileTransferResult.Successful)
                                _logger.LogInfo("Files transfer started successfully!");
                            else
                                _logger.LogError("Error trying to transfer files. Please, check that the source and destination folder paths are correct");
                        }
                        break;
                    case FileTransferCommand.Exit:
                        Stop();
                        break;
                    default:
                        _logger.LogError($"Bad formatted command line: {input}");
                        break;
                }
            }
        }

        public void Stop() => _cancellationTokenSource.Cancel();

        private void SetParserSettings(ParserSettings parserSettings)
        {
            parserSettings.AutoHelp = false;
            parserSettings.AutoVersion = false;
            parserSettings.CaseSensitive = false;
            parserSettings.IgnoreUnknownArguments = false;
        }

        private void WriteInstructionLines()
        {
            Console.WriteLine("Please, type one of the following commands:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("transfer -s \"source-folder-path\" -d \"target-folder-path\" (to transfer files from source folder to target folder)");
            Console.WriteLine("exit                                                     (to exit the App)");
            Console.ResetColor();
        }

        private FileTransferCommand ExtractActionFromCommand(string input)
        {
            if (string.IsNullOrEmpty(input))
                return FileTransferCommand.UnidentifiedCommand;

            var commandParams = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            bool commandIdentified = Enum.TryParse(commandParams.FirstOrDefault(), true, out FileTransferCommand command);
            return commandIdentified ? command : FileTransferCommand.UnidentifiedCommand;
        }

        private (string SourceFolder, string DestinationFolder) ExtractSourceAndDestinationFoldersFromCommand(string input)
        {   
            string sourceFolder = null;
            string destinationFolder = null;

            try
            {
                var commandParams = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var parserResult = _commandLineParser.ParseArguments<CommandLineOptions>(commandParams)
                    .WithParsed(options =>
                    {
                        sourceFolder = options.SourceFolderPath.Trim(' ', '"', '\\');
                        destinationFolder = options.DestinationFolderPath.Trim(' ', '"', '\\');
                    })
                    .WithNotParsed(errors =>
                    {
                        _logger.LogError($"Bad formatted command line: {input}");
                    });
            }
            catch (Exception error)
            {
                _logger.LogError(error.Message);
            }

            return (sourceFolder, destinationFolder);
        }

        private bool ValidFolderPaths(string sourceFolder, string destinationFolder)
        {
            bool validationResult = true;
            bool sourceFolderPathIsValid = ValidateFolderPath(sourceFolder);
            bool destinationFolderPathIsValid = ValidateFolderPath(destinationFolder);

            if (!sourceFolderPathIsValid)
            {
                validationResult = false;
                _logger.LogError($"ERROR: The source folder path could not be found");
            }
            if (!destinationFolderPathIsValid)
            {
                validationResult = false;
                _logger.LogError($"ERROR: The destination folder path could not be found");
            }

            return validationResult;
        }

        private bool ValidateFolderPath(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return false;

            return _fileSystemService.DirectoryExists(folderPath);
        }
    }
}
