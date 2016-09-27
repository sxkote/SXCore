namespace SXCore.Infrastructure.Values
{
    public class EmailServerConfig
    {
        public const int DefaultSMTPPort = 25;

        public string Server { get; set; }
        public int Port { get; set; }
        public bool SSL { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }

        public string SenderEmail { get; set; }
        public string SenderName { get; set; }

        public EmailServerConfig()
        {
            Port = DefaultSMTPPort;
            SSL = true;
        }
    }
}
