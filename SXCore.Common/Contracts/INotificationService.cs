using SXCore.Common.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Contracts
{
    public interface INotificationService
    {
        void SendNotification(string address, Message message, ICollection<FileData> files = null);
    }
}
