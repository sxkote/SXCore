using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Values
{
    public class FileUpload
    {
        private string _uid;
        private FileName _filename;
        private byte[] _data;
        private long _totalSize;

        public string UID
        { get { return _uid; } }

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

            _uid = Guid.NewGuid().ToString();

            _filename = file.FileName;
            _data = file.Data;
            _totalSize = file.Size;
        }

        public FileUpload(string uid, FileName filename, byte[] data = null, long totalSize = 0)
        {
            _uid = String.IsNullOrEmpty(uid) ? Guid.NewGuid().ToString() : uid;

            _filename = filename;
            _data = data;
            _totalSize = totalSize;
        }
    }
}
