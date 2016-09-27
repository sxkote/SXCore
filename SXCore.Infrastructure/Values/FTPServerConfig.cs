using SXCore.Common;
using SXCore.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Infrastructure.Values
{
    public class FTPServerConfig
    {
        public const int DefaultFTPPort = 21;
        public const int DefaultSFTPPort = 22;

        public string Server { get; set; }
        public int Port { get; set; }
        public bool SSL { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }

        public FTPServerConfig()
        {
            Port = DefaultFTPPort;
            SSL = false;
        }

        public FTPServerConfig(string connectionString)
            :this()
        {
            var connectionParams = connectionString.SplitParams(';');

            this.Server = connectionParams.GetValue("Server") ?? "";
            if (connectionParams.HasValue("Port"))
                this.Port = Convert.ToInt32(connectionParams.GetValue("Port")??DefaultFTPPort.ToString());
            if (connectionParams.HasValue("SSL"))
                this.SSL = connectionParams.GetValue("SSL").Equals("true", CommonService.StringComparison);

            this.Login = connectionParams.GetValue("Login") ?? "";
            this.Password = connectionParams.GetValue("Password") ?? "";
        }
    }
}
