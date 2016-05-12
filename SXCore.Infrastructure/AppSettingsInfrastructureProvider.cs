using SXCore.Common.Contracts;
using SXCore.Common.Values;
using SXCore.Infrastructure.Services.FileStorage;
using SXCore.Infrastructure.Services.Notifications;
using SXCore.Infrastructure.Values;

namespace SXCore.Infrastructure
{
    public class AppSettingsInfrastructureProvider : IInfrastructureProvider
    {
        private ISettingsProvider _settingsProvider;

        public ISettingsProvider SettingsProvider
        { get { return _settingsProvider; } }

        public IFileStorageService StorageService
        {
            get
            {
                var storageConfig = this.SettingsProvider.GetSettings("StorageConfig");

                FileStorageConfig config = Newtonsoft.Json.JsonConvert.DeserializeObject<FileStorageConfig>(storageConfig);

                switch (config.Type)
                {
                    case FileStorageConfig.StorageType.Local:
                        return new LocalFileStorageService(config);
                    case FileStorageConfig.StorageType.Hosting:
                        return new HostingFileStorageService(config);
                    case FileStorageConfig.StorageType.Azure:
                        return new AzureFileStorageService(config);
                }

                return null;
            }
        }

        public INotificationService EmailService
        {
            get
            {
                //var smtpConfig = System.Configuration.ConfigurationManager.AppSettings["SmtpConfig"];
                var smtpConfig = this.SettingsProvider.GetSettings("SmtpConfig");
                return new EmailNotificationService(smtpConfig);
            }
        }

        public INotificationService SmsService
        {
            get { return null; }
        }

        public AppSettingsInfrastructureProvider(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        public AppSettingsInfrastructureProvider()
        {
            _settingsProvider = new AppSettingsProvider();
        }

        public string GetSettings(string name)
        {
            return this.SettingsProvider == null ? "" : this.SettingsProvider.GetSettings(name);
        }

        public T GetSettings<T>(string name)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(this.GetSettings(name));
        }

        public Message GetMessageTemplate(string name)
        {
            return this.GetSettings<Message>(name);
        }
    }
}
