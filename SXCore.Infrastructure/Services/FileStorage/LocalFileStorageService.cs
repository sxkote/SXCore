using SXCore.Common.Contracts;
using SXCore.Infrastructure.Values;
using SXCore.Common.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SXCore.Infrastructure.Services.FileStorage
{
    public class LocalFileStorageService : IFileStorageService
    {
        protected string _root = "";

        public LocalFileStorageService()
        {
            _root = "";
        }

        public LocalFileStorageService(FileStorageConfig config)
        {
            if (String.IsNullOrEmpty(config.Root))
                _root = "";
            else
            {
                _root = config.Root.Replace("\\", "/");

                if (!_root.EndsWith("/"))
                    _root += "/";
            }
        }

        public LocalFileStorageService(string config)
            : this(Newtonsoft.Json.JsonConvert.DeserializeObject<FileStorageConfig>(config))
        { }

        #region Functions
        protected virtual string GetFullPath(string path)
        {
            return String.IsNullOrEmpty(_root) ? path : (_root + path);
        }

        protected async Task SaveDataToFileAsync(string path, byte[] data, FileMode fileMode = FileMode.OpenOrCreate)
        {
            var fullPath = this.GetFullPath(path);

            var info = new FileInfo(fullPath);

            var directory = info.DirectoryName;
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (var fs = File.Open(fullPath, fileMode))
                await fs.WriteAsync(data, 0, data.Length);
        }

        protected async Task<byte[]> ReadDataFromFileAsync(string path)
        {
            var fullPath = this.GetFullPath(path);
            if (!File.Exists(fullPath))
                return null;

            var fileInfo = new FileInfo(fullPath);

            byte[] result = new byte[fileInfo.Length];
            using (var fs = File.OpenRead(fullPath))
                await fs.ReadAsync(result, 0, result.Length);

            return result;
        }
        #endregion

        #region IFileStorageService
        public byte[] ReadFile(string path)
        {
            var task = Task.Run(() => this.ReadDataFromFileAsync(path));
            task.ConfigureAwait(false);
            return task.Result;
        }

        public async Task<byte[]> ReadFileAsync(string path)
        {
            return await this.ReadDataFromFileAsync(path);
        }

        public void SaveFile(string path, byte[] data)
        {
            var task = Task.Run(() => this.SaveDataToFileAsync(path, data, FileMode.Create));
            task.ConfigureAwait(false);
            task.Wait();
        }

        public async Task SaveFileAsync(string path, byte[] data)
        {
            await this.SaveDataToFileAsync(path, data, FileMode.OpenOrCreate);
        }

        public void AppendFile(string path, byte[] data, int chunkID = -1)
        {
            if (data == null || data.Length <= 0)
                return;

            var task = Task.Run(() => this.SaveDataToFileAsync(path, data, FileMode.Append));
            task.ConfigureAwait(false);
            task.Wait();
        }

        public async Task AppendFileAsync(string path, byte[] data, int chunkID = -1)
        {
            if (data == null || data.Length <= 0)
                return;

            await this.SaveDataToFileAsync(path, data, FileMode.Append);
        }

        public void CopyFile(string sourcePath, string destinationPath, bool deleteSource = false)
        {
            var sourceFullPath = this.GetFullPath(sourcePath);
            if (!File.Exists(sourceFullPath))
                return;

            var destFullPath = this.GetFullPath(destinationPath);
            File.Copy(sourceFullPath, destFullPath, true);

            if (deleteSource)
                File.Delete(sourceFullPath);
        }

        public void DeleteFile(string path)
        {
            var fullPath = this.GetFullPath(path);
            if (!File.Exists(fullPath))
                return;

            try { File.Delete(fullPath); }
            catch { }
        }

        public string HashMD5OfFile(string path)
        {
            return CommonService.GetMD5(this.ReadFile(path));
        }

        //public async Task<string> HashMD5OfFileAsync(string path)
        //{
        //    return await Task.Run<byte[]>(() => this.ReadDataFromFileAsync(path))
        //        .ContinueWith<string>(t => CommonService.GetMD5(t.Result));
        //}

        public long SizeOfFile(string path)
        {
            var fullPath = this.GetFullPath(path);
            if (!File.Exists(fullPath))
                return 0;

            var info = new FileInfo(fullPath);

            return info.Length;
        }

        public async Task ReadFileToStreamAsync(string path, Stream stream, long offset, long length)
        {
            var fullPath = this.GetFullPath(path);
            if (!File.Exists(fullPath))
                return;

            using (var fs = File.OpenRead(fullPath))
            {
                byte[] buffer = new byte[length];
                await fs.ReadAsync(buffer, (int)offset, (int)length);
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        public async Task ReadFileToStreamAsync(string path, Stream stream)
        {
            var fullPath = this.GetFullPath(path);
            if (!File.Exists(fullPath))
                return;

            using (var fs = File.OpenRead(fullPath))
            {
                await stream.CopyToAsync(stream);
            }
        }

        public virtual string GetFileUrl(string path, int hours = 4)
        {
            return this.GetFullPath(path);
        }
        #endregion
    }
}
