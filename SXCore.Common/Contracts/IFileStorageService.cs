using System.IO;
using System.Threading.Tasks;

namespace SXCore.Common.Contracts
{
    public interface IFileStorageService
    {
        byte[] ReadFile(string path);
        Task<byte[]> ReadFileAsync(string path);

        void SaveFile(string path, byte[] data);
        Task SaveFileAsync(string path, byte[] data);

        void AppendFile(string path, byte[] data);
        Task AppendFileAsync(string path, byte[] data);

        void CopyFile(string sourcePath, string destinationPath, bool deleteSource = false);

        void DeleteFile(string path);

        string HashMD5OfFile(string path);
        long SizeOfFile(string path);

        Task ReadFileToStreamAsync(string path, Stream stream);
        Task ReadFileToStreamAsync(string path, Stream stream, long offset, long length);

        string GetFileUrl(string path, int hours = 4);
    }
}
