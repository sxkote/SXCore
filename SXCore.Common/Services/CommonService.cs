using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Services
{
    public static class CommonService
    {
        [ThreadStatic]
        private static Random _randomizer;

        public static Random Randomizer
        {
            get
            {
                if (_randomizer == null)
                    _randomizer = new Random(DateTime.UtcNow.Millisecond);
                return _randomizer;
            }
        }

        static public DateTimeOffset Now
        { get { return TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time")); } }

        static public string HashPassword(string password)
        {
            return GetMD5(password);
        }

        static public bool VerifyPassword(string password, string hash)
        {
            return hash.Equals(GetMD5(password), StringComparison.InvariantCultureIgnoreCase);
        }

        static public string GenerateCode(string prefix = "", int length = 8)
        {
            var service = new Coder(36);

            string codePrefix = String.IsNullOrEmpty(prefix) ? "" : (prefix.TrimEnd('-') + "-");

            string codeDate = service.Encode(CommonService.Now.DateTime);

            string codeRandom = service.Generate(length <= 0 ? 8 : length);

            return String.Format("{0}{1}-{2}", codePrefix, codeDate, codeRandom);
        }

        static public string GeneratePassword(bool capitalize = true)
        {
            return Coder.Generate(8, Coder.MaxRadixLength, capitalize);
        }

        static public string GetMD5(byte[] data)
        {
            if (data == null || data.Length <= 0)
                return "";

            byte[] hash;

            using (var md5 = System.Security.Cryptography.MD5.Create())
                hash = md5.ComputeHash(data);

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sBuilder.Append(hash[i].ToString("x2"));

            return sBuilder.ToString();
        }

        static public string GetMD5(string text)
        {
            return GetMD5(Encoding.UTF8.GetBytes(text));
        }

        static public Task<string> GetMD5Async(byte[] data)
        {
            return Task.Run<string>(() => { return GetMD5(data); });
        }

        static public Task<string> GetMD5Async(string text)
        {
            return Task.Run<string>(() => { return GetMD5(text); });
        }

        static public long? IsNumber(object o)
        {
            if (o is long) return (long)o;
            if (o is int) return (long)(int)o;
            if (o is short) return (long)(short)o;
            if (o is byte) return (long)(byte)o;

            long id = 0;
            if (Int64.TryParse(o.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out id))
                return id;

            return null;
        }
    }
}
