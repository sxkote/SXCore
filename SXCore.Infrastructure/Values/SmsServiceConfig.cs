using SXCore.Common.Values;

namespace SXCore.Infrastructure.Values
{
    public class SmsServerConfig
    {
        public enum UrlMethod { GET, POST };

        public string RequestUrl { get; set; }
        public UrlMethod RequestMethod { get; set; }
        public ParamValueCollection RequestBodyParams { get; set; }

        public string SenderName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public ParamValueCollection Values { get; set; }

        public SmsServerConfig()
        {
            this.RequestMethod = UrlMethod.GET;
            this.Values = new ParamValueCollection();
            this.RequestBodyParams = new ParamValueCollection();
        }

        public ParamValueCollection GetValues()
        {
            var result = new ParamValueCollection();
            result.Add("login", this.Login);
            result.Add("password", this.Password);
            result.Add("sender", this.SenderName);
            result.AddRange(this.Values);
            return result;
        }
    }
}
