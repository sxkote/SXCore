using SXCore.Common.Contracts;
using SXCore.Common.Exceptions;
using SXCore.Common.Values;
using SXCore.Infrastructure.Services.FileStorage;
using SXCore.Infrastructure.Services.Notifications;
using SXCore.Infrastructure.Values;

namespace SXCore.Infrastructure.Services
{
    public class InfrastructureProvider : IInfrastructureProvider
    {
        public const string StorageConfigSettingsName = "StorageConfig";
        public const string EmailConfigSettingsName = "EmailConfig";

        protected ISettingsProvider _settingsProvider;
        protected IFileStorageService _storageService;
        protected INotificationService _emailService;
        protected INotificationService _smsService;

        public ISettingsProvider SettingsProvider
        { get { return _settingsProvider; } }

        public IFileStorageService StorageService
        {
            get
            {
                if (_storageService == null)
                    _storageService = this.CreateStorageService();

                return _storageService;
            }
        }

        public INotificationService EmailService
        {
            get
            {
                if (_emailService == null)
                    _emailService = this.CreateEmailService();

                return _emailService;
            }
        }

        public INotificationService SmsService
        {
            get
            {
                if (_smsService == null)
                    _smsService = this.CreateSmsService();

                return _smsService;
            }
        }

        public InfrastructureProvider(ISettingsProvider settings, IFileStorageService storage = null, INotificationService email = null, INotificationService sms = null)
        {
            if (settings == null)
                throw new CustomArgumentException("SettingsProvider can't be empty within InfrastructureProvider!");

            _settingsProvider = settings;
            _storageService = storage;
            _emailService = email;
            _smsService = sms;
        }

        public InfrastructureProvider()
            : this(new AppSettingsProvider())
        { }

        public string GetSettings(string name)
        {
            return this.SettingsProvider == null ? "" : this.SettingsProvider.GetSettings(name);
        }

        public T GetSettings<T>(string name)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(this.GetSettings(name));
        }

        public virtual Message GetMessageTemplate(string name)
        {
            return this.GetSettings<Message>(name);
        }

        protected virtual IFileStorageService CreateStorageService()
        {
            // get configuration from settings
            var config = this.GetSettings<FileStorageConfig>(StorageConfigSettingsName);

            // build Storage Service
            return BuildStorageService(config);
        }

        protected virtual INotificationService CreateEmailService()
        {
            // get configuration for Email Service
            var config = this.GetSettings<EmailServerConfig>(EmailConfigSettingsName);

            // create Email Service
            return new EmailNotificationService(config);
        }

        protected virtual INotificationService CreateSmsService()
        {
            return null;
        }

        static public IFileStorageService BuildStorageService(FileStorageConfig config)
        {
            if (config == null)
                throw new CustomArgumentException("Storage Config couldn't be empty!");

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
}
