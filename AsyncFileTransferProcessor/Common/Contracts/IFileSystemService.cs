namespace AsyncFileTransferProcessor.Common.Contracts
{
    public interface IFileSystemService
    {
        bool DirectoryExists(string path);

        string[] DirectoryFilesList(string path);

        bool FileExists(string path);

        string GetFileName(string path);

        string GetFileExtension(string path);

        string ComposePath(params string[] paths);

        void CopyFile(string filePath, string targetPath);
    }
}
