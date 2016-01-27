using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Infrastructure.Values
{
    public class FileStorageConfig
    {
        public enum StorageType { Local, Hosting, Azure };

        public StorageType Type { get; set; }
        public string ConnectionString { get; set; }
        public string Root { get; set; }

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
