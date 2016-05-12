using SXCore.Common.Values;
using System.Collections.Generic;

namespace SXCore.Common.Contracts
{
    public interface INotificationService
    {
        void SendNotification(string address, Message message, ICollection<FileData> files = null);
    }

    public interface IEmailNotificationService : INotificationService
    {
        void SendEmail(string email, string subject, string text, ICollection<FileData> files = null);
    }

    public interface ISmsNotificationService : INotificationService
    {
        void SendSms(string phone, string text);
    }
}
