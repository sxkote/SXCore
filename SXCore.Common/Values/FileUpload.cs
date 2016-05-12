using System;

namespace SXCore.Common.Values
{
    public class FileUpload
    {
        private string _uploadID;
        private int _chunkID;

        private FileName _filename;
        private byte[] _data;
        private long _totalSize;

        public string UploadID
        { get { return _uploadID; } }

        public int ChunkID
        { get { return _chunkID; } }

        public FileName FileName
        { get { return _filename; } }

        public byte[] Data
        { get { return _data; } }

        public string Hash
        { get { return _data == null ? "" : FileData.GetMD5(_data); } }

        public long Size
        { get { return _data == null ? 0 : _data.Length; } }

        public long TotalSize
        { get { return _totalSize <= 0 ? this.Size : _totalSize; } }

        public FileData FileData
        { get { return new FileData(this.FileName, this.Data); } }

        public bool IsComplete
        { get { return this.TotalSize <= this.Size; } }

        public FileUpload(FileData file)
        {
            if (file == null)
                throw new ArgumentNullException("FileData");

            _uploadID = Guid.NewGuid().ToString();

            _filename = file.FileName;
            _data = file.Data;
            _totalSize = file.Size;
        }

        public FileUpload(string uploadID, FileName filename, byte[] data = null, long totalSize = 0, int chunkID = -1)
        {
            _uploadID = String.IsNullOrEmpty(uploadID) ? Guid.NewGuid().ToString() : uploadID;
            _chunkID = chunkID;

            _filename = filename;
            _data = data;
            _totalSize = totalSize;
        }

        static public implicit operator FileUpload(FileData file)
        { return new FileUpload(file); }
    }
}
