using SXCore.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using SXCore.Infrastructure.Values;
using SXCore.Common;
using SXCore.Common.Exceptions;

namespace SXCore.Infrastructure.Services.FileStorage
{
    public class FTPFileStorageService : IFileStorageService
    {
        private FTPServerConfig _config;
        protected string _root = "";

        public string Root
        {
            get { return _root; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    _root = "";
                }
                else
                {
                    _root = value.Replace("\\", "/");
                    if (!_root.EndsWith("/"))
                        _root += "/";
                }
            }
        }

        public ILogger Logger
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public FTPFileStorageService(FTPServerConfig config, string root)
        {
            _config = config;
            this.Root = root;
        }

        public FTPFileStorageService(FileStorageConfig config)
        {
            _config = new FTPServerConfig(config.ConnectionString);
            this.Root = config.Root;
        }

        #region Functions
        protected string GetFilePath(string path)
        {
            if (String.IsNullOrWhiteSpace(_root))
                return path ?? "";

            return _root + (path ?? "");
        }

        protected string GetFullPath(string path)
        {
            return _config?.Server + "/" + this.GetFilePath(path);
        }

        protected FtpWebRequest CreateFTPRequest(string path, string method, bool useBinary = true)
        {
            var fullpath = this.GetFullPath(path);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(fullpath);
            request.Method = method;
            request.UseBinary = useBinary;
            request.Credentials = new NetworkCredential(_config.Login, _config.Password);

            return request;
        }

        protected void CreateDirectoryForFile(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                return;

            var index = path.LastIndexOf('/');
            if (index > 0)
            {
                var directoryPath = path.Substring(0, index);
                this.CreateDirectory(directoryPath);
            }
        }

        public bool CheckDirectoryExist(string directoryPath)
        {
            try
            {
                this.CreateFTPRequest(directoryPath, WebRequestMethods.Ftp.ListDirectory).GetResponse();
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    return false;
            }

            return true;
        }

        public void CreateDirectory(string directoryPath)
        {
            if (String.IsNullOrWhiteSpace(directoryPath))
                return;

            if (this.CheckDirectoryExist(directoryPath))
                return;

            var index = directoryPath.LastIndexOf('/');
            if (index > 0)
            {
                var subdirectoryPath = directoryPath.Substring(0, index);
                this.CreateDirectory(subdirectoryPath);
            }

            try
            {
                var response = this.CreateFTPRequest(directoryPath, WebRequestMethods.Ftp.MakeDirectory).GetResponse();
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                System.Diagnostics.Debug.WriteLine($"{response.StatusCode} : {response.StatusDescription}");
            }
        }

        public string ListDirectory(string directoryPath = "", bool details = false)
        {
            var result = new StringBuilder();

            var method = details ? WebRequestMethods.Ftp.ListDirectoryDetails : WebRequestMethods.Ftp.ListDirectory;

            using (var response = this.CreateFTPRequest(directoryPath, method).GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    result.AppendLine(line);
                    line = reader.ReadLine();
                }
            }

            return result.ToString();
        }

        public async Task UploadFileAsync(string path, byte[] data, string method = WebRequestMethods.Ftp.UploadFile)
        {
            if (data == null)
                return;

            this.CreateDirectoryForFile(path);

            var request = this.CreateFTPRequest(path, method);
            request.ContentLength = data.Length;

            Stream stream = await request.GetRequestStreamAsync();
            await stream.WriteAsync(data, 0, data.Length);
            stream.Close();

            var response = await request.GetResponseAsync();
            response.Close();
        }

        public void UploadFile(string path, byte[] data, string method = WebRequestMethods.Ftp.UploadFile)
        {
            if (data == null)
                return;

            this.CreateDirectoryForFile(path);

            var request = this.CreateFTPRequest(path, method);
            request.ContentLength = data.Length;

            Stream stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();

            var response = request.GetResponse();
            response.Close();
        }

        public async Task<byte[]> DownloadFileAsync(string path)
        {
            if (!this.Exist(path))
                return null;

            var request = this.CreateFTPRequest(path, WebRequestMethods.Ftp.DownloadFile);
            using (WebResponse response = await request.GetResponseAsync())
            using (Stream reader = response.GetResponseStream())
                return await reader.ReadFullyAsync();
        }

        public byte[] DownloadFile(string path)
        {
            if (!this.Exist(path))
                return null;

            var request = this.CreateFTPRequest(path, WebRequestMethods.Ftp.DownloadFile);
            using (WebResponse response = request.GetResponse())
            using (Stream reader = response.GetResponseStream())
                return reader.ReadFully();
        }
        #endregion

        #region IFileStorageService
        public void AppendFile(string path, byte[] data, int chunkID = -1)
        {
            this.UploadFile(path, data, WebRequestMethods.Ftp.AppendFile);
        }

        public async Task AppendFileAsync(string path, byte[] data, int chunkID = -1)
        {
            await this.UploadFileAsync(path, data, WebRequestMethods.Ftp.AppendFile);
        }

        public void CopyFile(string sourcePath, string destinationPath, bool deleteSource = false)
        {
            if (!this.Exist(sourcePath))
                throw new CustomArgumentException("Source file not exist!");

            var data = this.ReadFile(sourcePath);

            this.SaveFile(destinationPath, data);

            if (deleteSource)
                this.DeleteFile(sourcePath);
        }

        public void DeleteFile(string path)
        {
            if (!this.Exist(path))
                return;

            this.CreateFTPRequest(path, WebRequestMethods.Ftp.DeleteFile).GetResponse();
        }

        public bool Exist(string path)
        {
            var request = this.CreateFTPRequest(path, WebRequestMethods.Ftp.GetDateTimestamp);

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }
            }

            return true;
        }

        public string GetFileUrl(string path, int hours = 4)
        {
            return this.GetFullPath(path).Replace("ftp", "http");
        }

        public string HashMD5OfFile(string path)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadFile(string path)
        {
            return this.DownloadFile(path);
        }

        public async Task<byte[]> ReadFileAsync(string path)
        {
           return await this.DownloadFileAsync(path);
        }

        public async Task ReadFileToStreamAsync(string path, Stream stream)
        {
            var request = this.CreateFTPRequest(path, WebRequestMethods.Ftp.DownloadFile);
            using (WebResponse response = await request.GetResponseAsync())
            using (Stream reader = response.GetResponseStream())
            {
                await reader.CopyToAsync(stream);
            }
        }

        public async Task ReadFileToStreamAsync(string path, Stream stream, long offset, long length)
        {
            var request = this.CreateFTPRequest(path, WebRequestMethods.Ftp.DownloadFile);
            using (WebResponse response = await request.GetResponseAsync())
            using (Stream reader = response.GetResponseStream())
            {
                byte[] buffer = new byte[length];
                await reader.ReadAsync(buffer, (int)offset, (int)length);
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        public void SaveFile(string path, byte[] data)
        {
            this.UploadFile(path, data, WebRequestMethods.Ftp.UploadFile);
        }

        public async Task SaveFileAsync(string path, byte[] data)
        {
            await this.UploadFileAsync(path, data, WebRequestMethods.Ftp.UploadFile);
        }

        public long SizeOfFile(string path)
        {
            if (!this.Exist(path))
                return 0;

            var request = this.CreateFTPRequest(path, WebRequestMethods.Ftp.GetFileSize);
            using (WebResponse response = request.GetResponse())
                return response.ContentLength;
        }
        #endregion
    }
}
