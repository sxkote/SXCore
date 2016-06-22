using SXCore.Infrastructure.Values;
using System;

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
            : this(Newtonsoft.Json.JsonConvert.DeserializeObject<FileStorageConfig>(config))
        { }

        #region Functions
        protected override string GetFullPath(string path)
        {
            return HostingFileStorageService.MapPath(String.IsNullOrEmpty(_root) ? path : (_root + path));
        }

        public override string GetFileUrl(string path, int hours = 4)
        {
            return GetVirtualPath(String.IsNullOrEmpty(_root) ? path : (_root + path));
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
