using SXCore.Common.Contracts;

namespace SXCore.Infrastructure
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
