﻿using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SXCore.Common.Contracts;
using SXCore.Infrastructure.Values;
using SXCore.Common.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Infrastructure.Services.FileStorage
{
    public class AzureFileStorageService : IFileStorageService
    {
        public const string BlockFormat = "000000";

        protected readonly string _connectionString = "";
        protected readonly string _root = "";

        public AzureFileStorageService(FileStorageConfig config)
        {
            _connectionString = config.ConnectionString;
            _root = String.IsNullOrEmpty(config.Root) ? "" : config.Root.TrimEnd('/');
        }

        public AzureFileStorageService(string config)
            : this(Newtonsoft.Json.JsonConvert.DeserializeObject<FileStorageConfig>(config)) { }

        #region Functions
        protected CloudBlobClient GetStorageClient()
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            return storageAccount.CreateCloudBlobClient();
        }

        protected CloudBlobContainer GetContainer(string name)
        {
            return this.GetStorageClient().GetContainerReference(name);
        }

        public void DeleteContainer(string name)
        {
            var container = this.GetContainer(name);

            if (container != null)
                container.Delete();
        }

        public List<CloudBlobContainer> GetContainers(string filter = "")
        {
            return this.GetStorageClient().ListContainers()
                    .Where(c => String.IsNullOrEmpty(filter) || c.Name.Contains(filter))
                    .ToList();
        }

        public List<CloudBlockBlob> GetBlobs(CloudBlobContainer container, string filter = "")
        {
            return container.ListBlobs()
                    .OfType<CloudBlockBlob>()
                    .Where(b => String.IsNullOrEmpty(filter) || b.Name.Contains(filter))
                    .ToList();
        }

        public CloudBlockBlob GetBlobReference(string path)
        {
            string fullpath = String.IsNullOrEmpty(_root) ? path : String.Format("{0}/{1}", _root, path);

            var container = this.GetContainer(AzureFileStorageService.GetBlobContainer(fullpath));
            container.CreateIfNotExists(BlobContainerPublicAccessType.Off);

            return container.GetBlockBlobReference(AzureFileStorageService.GetBlobName(fullpath));
        }

        public string GetBlobUrl(string path)
        {
            return this.GetBlobReference(path).Uri.ToString();
        }

        public double GetSpaceUsed()
        {
            return this.GetContainers("")
                    .Sum(c => this.GetBlobs(c).Sum(b => b.Properties.Length));
        }

        protected string GetBlobBlockID(int blockID)
        {
            return Convert.ToBase64String(Encoding.Default.GetBytes(blockID.ToString(BlockFormat)));
        }
        #endregion

        #region IFileStorageService
        public byte[] ReadFile(string path)
        {
            var blob = this.GetBlobReference(path);
            if (blob == null || !blob.Exists())
                return null;

            byte[] result = new Byte[blob.Properties.Length];
            blob.DownloadToByteArray(result, 0);
            return result;
        }

        public async Task<byte[]> ReadFileAsync(string path)
        {
            var blob = this.GetBlobReference(path);
            if (blob == null || !blob.Exists())
                return null;

            byte[] result = new Byte[blob.Properties.Length];
            await blob.DownloadToByteArrayAsync(result, 0);
            return result;
        }

        public void SaveFile(string path, byte[] data)
        {
            var blob = this.GetBlobReference(path);

            var blockID = this.GetBlobBlockID(0);

            blob.PutBlock(blockID, new MemoryStream(data), null);

            blob.PutBlockList(new string[] { blockID });
        }

        public async Task SaveFileAsync(string path, byte[] data)
        {
            var blob = this.GetBlobReference(path);

            //var options = new BlobRequestOptions()
            //{
            //    MaximumExecutionTime = new TimeSpan(0, 5, 0),
            //    ServerTimeout = new TimeSpan(0, 5, 0)
            //};

            //await blob.UploadFromByteArrayAsync(data, 0, data.Length);

            //saving through the blocks give the opportunity to Append Blob
            var blockID = this.GetBlobBlockID(0);

            blob.PutBlock(blockID, new MemoryStream(data), null);

            await blob.PutBlockListAsync(new string[] { blockID });
        }

        public void AppendFile(string path, byte[] data)
        {
            if (data == null || data.Length <= 0)
                return;

            var blob = this.GetBlobReference(path);
            if (blob == null)
                return;

            var blockIDs = new List<string>();
            if (blob.Exists())
                blockIDs = (blob.DownloadBlockList()).Select(b => b.Name).ToList();

            var newBlockID = this.GetBlobBlockID(blockIDs.Count);

            blob.PutBlock(newBlockID, new MemoryStream(data), null);

            blockIDs.Add(newBlockID);

            blob.PutBlockList(blockIDs);
        }

        public async Task AppendFileAsync(string path, byte[] data)
        {
            if (data == null || data.Length <= 0)
                return;

            var blob = this.GetBlobReference(path);
            if (blob == null)
                return;

            var blockIDs = new List<string>();
            if (blob.Exists())
                blockIDs = (await blob.DownloadBlockListAsync()).Select(b => b.Name).ToList();

            var newBlockID = this.GetBlobBlockID(blockIDs.Count);

            blob.PutBlock(newBlockID, new MemoryStream(data), null);

            blockIDs.Add(newBlockID);

            await blob.PutBlockListAsync(blockIDs);
        }

        public void DeleteFile(string path)
        {
            var blob = this.GetBlobReference(path);
            if (blob != null && blob.Exists())
                blob.Delete();
        }

        public void CopyFile(string sourcePath, string destinationPath, bool deleteSource = false)
        {
            var source = this.GetBlobReference(sourcePath);
            if (source == null && !source.Exists())
                return;

            var destination = this.GetBlobReference(destinationPath);

            //destination.BeginStartCopyFromBlob(source, CopyFileCallback, deleteSource ? sourcePath : "");
            destination.BeginStartCopy(source, CopyFileCallback, deleteSource ? sourcePath : "");
        }

        // The following method is called when each asynchronous operation completes.
        private void CopyFileCallback(IAsyncResult result)
        {
            string sourcePath = result.AsyncState as string;
            if (result.IsCompleted && !String.IsNullOrEmpty(sourcePath))
            {
                try { this.DeleteFile(sourcePath); }
                catch { }
            }
        }

        public string HashMD5OfFile(string path)
        {
            var blob = this.GetBlobReference(path);
            if (blob == null || !blob.Exists())
                return "";

            return blob.Properties.ContentMD5;

            //byte[] content = this.ReadFile(path);
            //var result = CommonService.GetMD5(content);
            //blob.Properties.ContentMD5 = result;

            //return result;
        }

        //public async Task<string> HashMD5OfFileAsync(string path)
        //{
        //    var blob = this.GetBlobReference(path);
        //    if (blob == null || !blob.Exists())
        //        return "";

        //    byte[] content = await this.ReadFileAsync(path);
        //    var result = await CommonService.GetMD5Async(content);
        //    blob.Properties.ContentMD5 = result;

        //    return result;
        //}

        public long SizeOfFile(string path)
        {
            var blob = this.GetBlobReference(path);
            if (blob == null || !blob.Exists())
                return 0;

            return blob.Properties.Length;
        }

        public async Task ReadFileToStreamAsync(string path, Stream stream, long offset, long length)
        {
            var blob = this.GetBlobReference(path);
            if (blob == null || !blob.Exists())
                return;

            await blob.DownloadRangeToStreamAsync(stream, offset, length);
        }

        public async Task ReadFileToStreamAsync(string path, Stream stream)
        {
            var blob = this.GetBlobReference(path);
            if (blob == null || !blob.Exists())
                return;

            await blob.DownloadToStreamAsync(stream);
        }

        public string GetFileUrl(string path, int hours = 4)
        {
            var blob = this.GetBlobReference(path);
            if (blob == null || !blob.Exists())
                return "";

            var sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5);
            sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(hours);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read;

            //Generate the shared access signature on the blob, setting the constraints directly on the signature.
            string sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

            //Return the URI string for the container, including the SAS token.
            return blob.Uri + sasBlobToken;
        }
        #endregion

        #region Statics
        static protected string GetBlobContainer(string path)
        {
            if (path.Contains('/'))
                return path.Substring(0, path.IndexOf('/')).ToLower();
            return "null";
        }

        static protected string GetBlobName(string path)
        {
            if (path.Contains('/'))
                return path.Substring(path.IndexOf('/') + 1);
            return path.ToLower();
        }
        #endregion
    }
}
