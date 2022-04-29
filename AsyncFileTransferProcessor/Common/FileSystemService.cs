using AsyncFileTransferProcessor.Common.Contracts;
using System.IO;

namespace AsyncFileTransferProcessor.Common
{
    public class FileSystemService : IFileSystemService
    {
        public bool DirectoryExists(string path) => Directory.Exists(path);

        public string[] DirectoryFilesList(string path) => Directory.GetFiles(path);

        public bool FileExists(string path) => File.Exists(path);

        public string GetFileName(string path) => Path.GetFileName(path);

        public string GetFileExtension(string path) => Path.GetExtension(path);

        public string ComposePath(params string[] paths) => Path.Combine(paths);

        public void CopyFile(string filePath, string targetPath) => File.Copy(filePath, targetPath, true);
    }
}
