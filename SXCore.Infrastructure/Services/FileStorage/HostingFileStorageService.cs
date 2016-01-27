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
    public class HostingFileStorageService : LocalFileStorageService
    {
        public HostingFileStorageService()
        {
            _root = "~/";
        }

        public HostingFileStorageService(FileStorageConfig config)
        {
            if (String.IsNullOrEmpty(config.Root))
                _root = "~/";
            else
            {
                _root = HostingFileStorageService.GetVirtualPath(config.Root);

                if (!_root.EndsWith("/"))
                    _root += "/";
            }
        }

        public HostingFileStorageService(string config)
            : this(Newtonsoft.Json.JsonConvert.DeserializeObject<FileStorageConfig>(config)) { }

        #region Functions
        protected override string GetFullPath(string path)
        {
            return HostingFileStorageService.MapPath(String.IsNullOrEmpty(_root) ? path : (_root + path));
        }
        #endregion

        #region Statics
        static public string GetVirtualPath(string path)
        {
            return "~/" + path.Replace("\\", "/").TrimStart('~', '/');
        }

        static public string MapPath(string path)
        {
            return System.Web.Hosting.HostingEnvironment.MapPath(path);
        }
        #endregion
    }
}
