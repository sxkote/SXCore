using SXCore.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Infrastructure.Services
{
    public class AppSettingsProvider : ISettingsProvider
    {
        public string this[string name]
        { get { return this.GetSettings(name); } }

        public string GetSettings(string name)
        {
            return System.Configuration.ConfigurationManager.AppSettings[name];
        }
    }
}
