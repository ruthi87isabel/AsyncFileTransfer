namespace AsyncFileTransferProcessor.Contracts.Models
{
    public class FileTransferInfo
    {
        public string FilePath { get; set; }
        public string FileExtension { get; set; }
        public string TargetFolderPath { get; set; }
    }
}
