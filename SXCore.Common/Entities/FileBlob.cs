using SXCore.Common.Interfaces;
using SXCore.Common.Services;
using SXCore.Common.Values;
using System;

namespace SXCore.Common.Entities
{
    public class FileBlob : Entity, ICoded
    {
        public string Code { get; protected set; }

        public string Folder { get; protected set; }
        public FileName FileName { get; protected set; }

        public DateTimeOffset Date { get; protected set; }
        public string Hash { get; protected set; }
        public long Size { get; protected set; }

        private FileBlob()
        {
            this.Code = Guid.NewGuid().ToString();
        }

        public override string ToString()
        { return this.FileName.Name; }

        public override bool Equals(object entity)
        {
            //Check whether the compared object is null. 
            if (Object.ReferenceEquals(entity, null)) return false;

            //Check whether the compared object references the same data. 
            if (Object.ReferenceEquals(this, entity)) return true;

            var file = entity as FileBlob;
            if (!Object.ReferenceEquals(file, null))
            {
                if (this.ID == file.ID) return true;

                return this.Code.Equals(file.Code);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Code.GetHashCode();
        }

        public string GetPath()
        {
            return $"{this.Folder}{this.Code}{this.FileName.Extension}".ToLower();
        }

        static public FileBlob Create(string folder, string filename, string hash, long size)
        {
            return new FileBlob()
            {
                Folder = String.IsNullOrEmpty(folder) ? "" : folder,
                FileName = filename ?? "file",
                Date = CommonService.Now,
                Hash = hash ?? "",
                Size = size
            };
        }
    }

}
