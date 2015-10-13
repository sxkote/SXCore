using SXCore.Common.Contracts;
using SXCore.Common.Infrastructure.Values;
using SXCore.Common.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Infrastructure.Services.Notifications
{
    public sealed class EmailNotificationService : INotificationService
    {
        private EmailServerConfig _config;

        private SmtpClient Server
        {
            get
            {
                if (_config == null)
                    return null;

                SmtpClient result = ((_config.Port <= 0) ? new SmtpClient(_config.Server) : new SmtpClient(_config.Server, _config.Port));
                result.UseDefaultCredentials = false;
                result.Credentials = new NetworkCredential(_config.Login, _config.Password);
                result.EnableSsl = _config.SSL;
                result.DeliveryMethod = SmtpDeliveryMethod.Network;
                result.Timeout = 15 * 1000;

                //result.DeliveryFormat = SmtpDeliveryFormat.International;

                return result;
            }
        }

        private MailAddress Sender
        {
            get
            {
                if (_config == null)
                    return null;

                var address = String.IsNullOrEmpty(_config.SenderEmail) ? _config.Login : _config.SenderEmail;
                var name = String.IsNullOrEmpty(_config.SenderName) ? address : _config.SenderName;

                return new MailAddress(address, name);
            }
        }

        public EmailNotificationService(EmailServerConfig config)
        { _config = config; }

        public EmailNotificationService(string config)
            : this(Newtonsoft.Json.JsonConvert.DeserializeObject<EmailServerConfig>(config)) { }

        public void SendEmail(string toEmail, string subject, string body, ICollection<FileData> files = null)
        {
            if (_config == null)
                return;

            var message = new MailMessage(_config.SenderEmail, toEmail, subject, body)
            {
                Sender = this.Sender
            };

            if (files != null && files.Count > 0)
                foreach (var file in files)
                    if (file.Data != null && file.Data.Length > 0)
                        message.Attachments.Add(new Attachment(new MemoryStream(file.Data), file.FileName.Name, file.FileName.MimeType));

            this.Server.Send(message);
        }

        public void SendNotification(string address, Message message, ICollection<FileData> files = null)
        {
            this.SendEmail(address, message.Subject, message.Text, files);
        }
    }
}
