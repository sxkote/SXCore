using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using SXCore.Common.Contracts;
using SXCore.Common.Exceptions;
using SXCore.Common.Services;
using SXCore.Infrastructure.Values;
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
        public const int MaxUploadBlobSize = 4 * 1024 * 1024;

        protected readonly string _connectionString = "";
        protected string _root = "";

        public string Root
        {
            get { return _root; }
            set { _root = String.IsNullOrEmpty(value) ? "" : value.TrimEnd('/'); }
        }

        public ILogger Logger { get; set; }

        public AzureFileStorageService(FileStorageConfig config)
        {
            _connectionString = config.ConnectionString;
            this.Root = config.Root;
        }

        public AzureFileStorageService(string connectionString, string root = "")
        {
            _connectionString = connectionString;
            this.Root = root;
        }

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
            string fullpath = String.IsNullOrEmpty(this.Root) ? path : String.Format("{0}/{1}", this.Root, path);

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
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(blockID.ToString(BlockFormat)));
        }

        protected IEnumerable<string> GetBlobBlocksOrdered(IEnumerable<string> blocks)
        {
            if (blocks == null)
                return null;

            var blockItems = blocks.Select(value => new
            {
                position = Convert.ToInt32(Encoding.UTF8.GetString(Convert.FromBase64String(value))),
                name = value
            });

            return blockItems.OrderBy(i => i.position).Select(i => i.name);
        }

        protected void LogMessage(string message)
        {
            if (this.Logger != null)
                this.Logger.Log("  AzureStorageService:: " + message ?? "");
        }

        public void AllowCORS()
        {
            var client = this.GetStorageClient();

            var serviceProperties = client.GetServiceProperties();

            serviceProperties.Cors.CorsRules.Clear();

            serviceProperties.Cors.CorsRules.Add(new CorsRule()
            {
                AllowedHeaders =  { "*" },
                AllowedMethods = CorsHttpMethods.Get | CorsHttpMethods.Head,
                AllowedOrigins = { "*" },
                ExposedHeaders = { "*" },
                MaxAgeInSeconds = 600
            });

            client.SetServiceProperties(serviceProperties);
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

        //public void SaveFile(string path, byte[] data)
        //{
        //    if (data == null)
        //        throw new CustomArgumentException("NULL Data can't be written to Storage!");

        //    var blob = this.GetBlobReference(path);

        //    List<string> blocks = new List<string>();

        //    using (MemoryStream ms = new MemoryStream(data))
        //    {
        //        byte[] buffer = new byte[MaxUploadBlobSize];
        //        int index = 0, offset = 0, readed = 0;

        //        this.LogMessage($"begin uploading to {path} [{data.Length}]");

        //        // reading portions from the array
        //        while ((readed = ms.Read(buffer, offset, MaxUploadBlobSize)) > 0)
        //        {
        //            // move through array
        //            offset += readed;

        //            // generate new blockID
        //            var blockID = this.GetBlobBlockID(index);

        //            // put block with buffer into storage
        //            blob.PutBlock(blockID, new MemoryStream(buffer), null);

        //            // remember blockID to be saved
        //            blocks.Add(blockID);

        //            this.LogMessage($"uploaded block {index}:{blockID} [{buffer.Length}]");

        //            // increase index to get next blockID
        //            index++;
        //        }
        //    }

        //    // write to storage
        //    blob.PutBlockList(blocks);

        //    this.LogMessage($"complete uploading to {path} [{data.Length}] with {blocks.Count} blocks");
        //}

        public void SaveFile(string path, byte[] data)
        {
            var task = Task.Run(() => this.SaveFileAsync(path, data));
            task.ConfigureAwait(false);
            task.Wait();
        }

        public async Task SaveFileAsync(string path, byte[] data)
        {
            if (data == null)
                throw new CustomArgumentException("NULL Data can't be written to Storage!");

            var blob = this.GetBlobReference(path);

            List<string> blocks = new List<string>();

            using (MemoryStream ms = new MemoryStream(data))
            {
                byte[] buffer = new byte[MaxUploadBlobSize];
                int index = 0, offset = 0, readed = 0;

                this.LogMessage($"begin uploading to {path} [{data.Length}]");

                // reading portions from the array
                while ((readed = await ms.ReadAsync(buffer, 0, MaxUploadBlobSize)) > 0)
                {
                    // move through array
                    offset += readed;

                    // generate new blockID
                    var blockID = this.GetBlobBlockID(index);

                    // put block with buffer into storage
                    await blob.PutBlockAsync(blockID, new MemoryStream(buffer, 0, readed), null);

                    // remember blockID to be saved
                    blocks.Add(blockID);

                    this.LogMessage($"uploaded block {index}:{blockID} [{readed}]");

                    // increase index to get next blockID
                    index++;
                }
            }

            // write to storage
            await blob.PutBlockListAsync(blocks);

            this.LogMessage($"complete uploading to {path} [{data.Length}] with {blocks.Count} blocks");
        }

        public void AppendFile(string path, byte[] data, int chunkID = -1)
        {
            if (data == null || data.Length <= 0)
                return;

            var blob = this.GetBlobReference(path);
            if (blob == null)
                return;

            var blockIDs = new List<string>();
            if (blob.Exists())
                blockIDs = (blob.DownloadBlockList()).Select(b => b.Name).ToList();

            var newBlockID = this.GetBlobBlockID(chunkID < 0 ? blockIDs.Count : chunkID);

            blob.PutBlock(newBlockID, new MemoryStream(data), null);

            if (!blockIDs.Contains(newBlockID))
                blockIDs.Add(newBlockID);

            blob.PutBlockList(this.GetBlobBlocksOrdered(blockIDs));
        }

        public async Task AppendFileAsync(string path, byte[] data, int chunkID = -1)
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
            if (sourcePath.Equals(destinationPath, CommonService.StringComparison))
                return;

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

        public bool Exist(string path)
        {
            var blob = this.GetBlobReference(path);
            return blob != null && blob.Exists();
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

        static public string CreateAzureSASToken(string connection, int hours = 5)
        {
            // Create Azure Account with Connection String
            CloudStorageAccount account = CloudStorageAccount.Parse(connection);

            // Create a new access policy for the account.
            SharedAccessAccountPolicy policy = new SharedAccessAccountPolicy()
            {
                Permissions = SharedAccessAccountPermissions.Read | SharedAccessAccountPermissions.List,
                Services = SharedAccessAccountServices.Blob,
                ResourceTypes = SharedAccessAccountResourceTypes.Object,
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(5),
                Protocols = SharedAccessProtocol.HttpsOrHttp
            };

            // Return the SAS token.
            return account.GetSharedAccessSignature(policy);
        }
        #endregion
    }
}
