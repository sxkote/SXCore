using SXCore.Common.Contracts;

namespace SXCore.Infrastructure.Services
{
    public class AppSettingsProvider : ISettingsProvider
    {
        public string this[string name]
        { get { return this.GetSettings(name); } }

        public string GetSettings(string name)
        {
            return System.Configuration.ConfigurationManager.AppSettings[name];
        }

        static public string GetConnectionString(string name)
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
        }
    }
}
