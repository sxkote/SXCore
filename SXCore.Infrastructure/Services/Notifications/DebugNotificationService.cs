using SXCore.Common.Contracts;
using SXCore.Common.Values;
using System.Collections.Generic;
using System.Diagnostics;

namespace SXCore.Infrastructure.Services.Notifications
{
    public class DebugNotificationService : INotificationService
    {
        public void SendNotification(string address, Message message, ICollection<FileData> files = null)
        {
            Debug.WriteLine($"DEBUG-NOTIFICATION: to = {address ?? ""}; subject = {(message == null ? "NULL" : message.Subject)}; text = {(message == null ? "NULL" : message.Text)};");
        }
    }
}
