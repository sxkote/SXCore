using SXCore.Common.Values;

namespace SXCore.Common.Contracts
{
    public interface IInfrastructureProvider
    {
        IFileStorageService StorageService { get; }

        INotificationService EmailService { get; }

        INotificationService SmsService { get; }

        string GetSettings(string name);

        T GetSettings<T>(string name);

        Message GetMessageTemplate(string name);
    }
}
