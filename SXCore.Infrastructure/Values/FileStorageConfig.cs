using SXCore.Common;
using SXCore.Common.Values;

namespace SXCore.Infrastructure.Values
{
    public class FileStorageConfig
    {
        public enum StorageType { Local, Hosting, Azure, FTP };

        public StorageType Type { get; set; }
        public string ConnectionString { get; set; }
        public string Root { get; set; }

        public ParamValueCollection ConnectionParams
        {
            get { return this.ConnectionString.SplitParams(); }
        }

        public FileStorageConfig()
        {
            this.Type = StorageType.Local;
            this.ConnectionString = "";
            this.Root = "";
        }

        public FileStorageConfig(StorageType type, string connection = "", string root = "")
        {
            this.Type = type;
            this.ConnectionString = connection;
            this.Root = root;
        }
    }
}
