using SXCore.Common.Contracts;
using SXCore.Common.Exceptions;
using SXCore.Common.Interfaces;
using SXCore.Common.Values;
using SXCore.Domain.Contracts;
using SXCore.Domain.Entities;
using SXCore.Domain.Values;
using System;
using System.Threading.Tasks;

namespace SXCore.Domain.Managers
{
    public abstract class CoreManager<TUnitOfWork> : ICoreManager
       where TUnitOfWork : class, ICoreUnitOfWork
    {
        public event EventHandler<EventArgument<UploadFileDataInfo>> BeforeFileDataSave;
        public event EventHandler<EventArgument<UploadChunksDataInfo>> BeforeFileChunksSave;

        protected TUnitOfWork _uow;

        public TUnitOfWork UnitOfWork { get { return _uow; } }

        protected abstract IFileStorageService FileStorageService { get; }
        protected abstract ITokenProvider TokenProvider { get; }

        public Token Token
        {
            get
            {
                return this.TokenProvider?.GetToken();
            }
        }

        public CoreManager(TUnitOfWork uow)
        {
            _uow = uow;
        }

        public void SaveChanges()
        {
            this.UnitOfWork.SaveChanges();
        }

        #region Checking Rights
        protected bool CheckRights(string role)
        {
            var token = this.Token;
            if (token == null)
                throw new CustomAuthenticationException();

            return token.IsInRole(role);
        }

        protected bool CheckRights(long subscriptionID)
        {
            var token = this.Token;
            if (token == null)
                throw new CustomAuthenticationException();

            if (token.IsAdministrator() || token.IsSupervisor())
                return true;

            if (token.SubscriptionID <= 0)
                return false;

            return token.SubscriptionID == subscriptionID;
        }

        protected bool CheckRights(ISubscriptioned item)
        {
            return item == null || this.CheckRights(item.SubscriptionID);
        }
        #endregion

        #region Avatar
        protected virtual string GetAvatarPath(string code)
        {
            return Avatar.GetAvatarPath(code);
        }

        protected Avatar SaveAvatar(FileData file, int maxSize = 100)
        {
            if (file == null || file.Data == null)
                return null;

            return this.SaveAvatar(file.Data, maxSize);
        }

        protected Avatar SaveAvatar(byte[] data, int maxSize = 100)
        {
            if (this.FileStorageService == null)
                return null;

            var avatarFile = Avatar.MakeAvatarFile(data, maxSize);
            if (avatarFile == null || avatarFile.Data == null)
                return null;

            var avatar = Avatar.CreateAvatar();

            var path = this.GetAvatarPath(avatar.Code);

            this.FileStorageService.SaveFile(path, avatarFile.Data);

            return this.UnitOfWork.Create(avatar);
        }

        protected FileData ReadAvatar(string code)
        {
            if (this.FileStorageService == null)
                return null;

            var path = this.GetAvatarPath(code);

            var data = this.FileStorageService.ReadFile(path);

            return new FileData(Avatar.FileName, data);
        }

        protected void DeleteAvatar(string code)
        {
            if (this.FileStorageService == null)
                return;

            var path = this.GetAvatarPath(code);

            this.FileStorageService.DeleteFile(path);
        }
        #endregion

        #region Files
        protected virtual string GetChunksUploadPath(string uploadID)
        {
            return $"chunks/file-{uploadID}".ToLower();
        }

        protected virtual string GetFileBlobPath(FileBlob blob)
        {
            return blob?.GetPath();
        }

        protected virtual bool IsFileSizeTooBigToComputeMD5(long size)
        {
            return size > 5 * 1024 * 1024;
        }

        public void OnBeforeFileDataSave(FileData fileData)
        {
            if (this.BeforeFileDataSave != null)
                this.BeforeFileDataSave(this, new EventArgument<UploadFileDataInfo>(new UploadFileDataInfo(fileData)));
        }

        public void OnBeforeFileChunksSave(string chunksPath, FileUpload fileUpload)
        {
            if (this.BeforeFileChunksSave != null)
                this.BeforeFileChunksSave(this, new EventArgument<UploadChunksDataInfo>(new UploadChunksDataInfo(chunksPath, fileUpload)));
        }

        public FileBlob SaveFile(FileData file, string folder = "")
        {
            if (this.FileStorageService == null)
                return null;

            if (file == null || file.Data == null || file.Size <= 0)
                return null;

            //var processed = this.ProcessFileBeforeSave(file);

            this.OnBeforeFileDataSave(file);

            // create FileBlob Entity
            var blob = FileBlob.Create(folder, file.FileName, file.Hash, file.Size);

            // get path of the File
            var path = this.GetFileBlobPath(blob);

            // save File to StorageService
            this.FileStorageService.SaveFile(path, file.Data);

            // save FileBlob Entity
            return this.UnitOfWork.Create(blob);
        }

        protected FileBlob UploadFile(FileUpload upload, string folder = "")
        {
            if (this.FileStorageService == null)
                return null;

            if (upload == null || upload.TotalSize <= 0)
                return null;

            // immediately save complete upload data
            if (upload.IsComplete)
                return this.SaveFile(upload.FileData, folder);

            // get chunks path
            string chunksPath = this.GetChunksUploadPath(upload.UploadID);

            // append chunks to temporary file
            if (upload.Data != null && upload.Data.Length > 0)
                this.FileStorageService.AppendFile(chunksPath, upload.Data, upload.ChunkID);

            // if upload is finished
            if (upload.TotalSize <= this.FileStorageService.SizeOfFile(chunksPath))
            {
                // pre Process uploaded file
                this.OnBeforeFileChunksSave(chunksPath, upload);

                // define uploaded size
                long chunksSize = this.FileStorageService.SizeOfFile(chunksPath);

                // get newly uploaded hash
                string hash = "";
                if (!this.IsFileSizeTooBigToComputeMD5(chunksSize))
                    hash = this.FileStorageService.HashMD5OfFile(chunksPath);

                // create FileBlob Entity
                var blob = FileBlob.Create(folder, upload.FileName, hash, chunksSize);

                var path = this.GetFileBlobPath(blob);

                // save File to StorageService
                this.FileStorageService.CopyFile(chunksPath, path, true);

                // save FileBlob Entity
                return this.UnitOfWork.Create(blob);
            }

            return null;
        }

        protected FileData ReadFile(object key)
        {
            var task = Task.Run(() => this.ReadFileAsync(key));
            task.ConfigureAwait(false);
            return task.Result;
        }

        protected async Task<FileData> ReadFileAsync(object key)
        {
            if (this.FileStorageService == null)
                return null;

            var blob = this.UnitOfWork.Find<FileBlob>(key);
            if (blob == null || String.IsNullOrEmpty(blob.Code))
                return null;

            var path = this.GetFileBlobPath(blob);

            var data = await this.FileStorageService.ReadFileAsync(path);

            return new FileData(blob.FileName, data);
        }
        #endregion

        public void Dispose()
        {
            var uowDisposable = _uow as IDisposable;
            if (uowDisposable != null)
                uowDisposable.Dispose();
        }
    }
}
