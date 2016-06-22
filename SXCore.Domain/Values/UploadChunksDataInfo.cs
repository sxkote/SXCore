using SXCore.Common.Exceptions;
using SXCore.Common.Values;

namespace SXCore.Domain.Values
{
    public class UploadChunksDataInfo
    {
        public string Path { get; set; }
        public FileUpload Upload { get; set; }

        public FileName FileName { get { return this.Upload.FileName; } }
        public long TotalSize { get { return this.Upload.TotalSize; } }

        public UploadChunksDataInfo(string chunksPath, FileUpload fileUpload)
        {
            if (fileUpload == null)
                throw new CustomArgumentException("Valid FileUpload data should be provided to UploadChunksDataInfo structure!");

            this.Path = chunksPath;
            this.Upload = fileUpload;
        }
    }
}
