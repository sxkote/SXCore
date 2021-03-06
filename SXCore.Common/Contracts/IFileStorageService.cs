﻿using System.IO;
using System.Threading.Tasks;

namespace SXCore.Common.Contracts
{
    public interface IFileStorageService
    {
        ILogger Logger { get; set; }

        byte[] ReadFile(string path);
        Task<byte[]> ReadFileAsync(string path);

        void SaveFile(string path, byte[] data);
        Task SaveFileAsync(string path, byte[] data);

        void AppendFile(string path, byte[] data, int chunkID = -1);
        Task AppendFileAsync(string path, byte[] data, int chunkID = -1);

        void CopyFile(string sourcePath, string destinationPath, bool deleteSource = false);

        void DeleteFile(string path);

        string HashMD5OfFile(string path);
        long SizeOfFile(string path);
        bool Exist(string path);

        Task ReadFileToStreamAsync(string path, Stream stream);
        Task ReadFileToStreamAsync(string path, Stream stream, long offset, long length);

        string GetFileUrl(string path, int hours = 4);
    }
}
