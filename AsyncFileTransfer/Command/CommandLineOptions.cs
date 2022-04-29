using CommandLine;

namespace AsyncFileTransfer.Command
{
    public class CommandLineOptions
    {
        [Option('s', "src", Required = true, HelpText = "Source folder path")]
        public string SourceFolderPath { get; set; }

        [Option('d', "dest", Required = true, HelpText = "Destination folder path")]
        public string DestinationFolderPath { get; set; }
    }
}
