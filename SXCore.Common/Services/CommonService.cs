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

        //static public string GeneratePassword(int length)
        //{
        //    return "qwerty";
        //}

        static public string HashPassword(string password, int level = 10)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, level);
        }

        static public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        static public string GenerateCode(string prefix = "", byte subscriptionID = 0, int length = 6)
        {
            var service = new CoderService(36);

            string codePrefix = String.IsNullOrEmpty(prefix) ? "" : (prefix.TrimEnd('-') + "-");

            string codeSubscr = subscriptionID <= 0 ? "" : (subscriptionID.ToString() + "-");
            if (String.IsNullOrEmpty(codePrefix) && !String.IsNullOrEmpty(codeSubscr))
                codeSubscr = "S" + codeSubscr;

            string codeDate = service.Encode(CommonService.Now.DateTime);

            string codeRandom = service.Generate(length <= 0 ? 6 : length);

            return String.Format("{0}{1}{2}-{3}", codePrefix, codeSubscr, codeDate, codeRandom);
        }

        static public string GenerateEntityCode(string prefix = "", int length = 8)
        {
            var service = new CoderService(36);

            string codePrefix = String.IsNullOrEmpty(prefix) ? "" : (prefix.TrimEnd('-') + "-");

            string codeDate = service.Encode(CommonService.Now.DateTime);

            string codeRandom = service.Generate(length <= 0 ? 8 : length);

            return String.Format("{0}{1}-{2}", codePrefix, codeDate, codeRandom);
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

        //static public string ReplaceParams(this string text, ValuesCollection collection)
        //{
        //    if (String.IsNullOrEmpty(text))
        //        return "";

        //    var result = Regex.Replace(text, @"#(?<paramname>[\w\-\.]+)", match =>
        //    {
        //        string paramname = match.Groups["paramname"].Value;

        //        var value = collection.GetValue(paramname);

        //        return value == null ? "" : value.ToString();
        //    });

        //    return result;
        //}

        //static public ICollection<T> Merge<T, U>(this ICollection<T> baseCollection, ICollection<U> mergeCollection, Action<T, U> onMerge = null, Action<T> onAdd = null, Action<T> onDelete = null, Func<T, U, bool> compare = null)
        //    where T : class, new()
        //    where U : class
        //{
        //    if (mergeCollection == null)
        //        return baseCollection;

        //    var baseList = baseCollection.ToList();
        //    for (int i = baseList.Count - 1; i >= 0; i--)
        //    {
        //        var currentItem = baseList[i];

        //        var mergeItem = mergeCollection.SingleOrDefault(item => compare == null ? currentItem.Equals(item) : compare(currentItem, item));

        //        if (mergeItem == null)
        //        {
        //            baseCollection.Remove(currentItem);

        //            if (onDelete != null)
        //                onDelete(currentItem);
        //        }
        //        else if (onMerge != null)
        //            onMerge(currentItem, mergeItem);
        //    }

        //    var mergeList = mergeCollection.ToList();
        //    for (int i = 0; i < mergeList.Count; i++)
        //    {
        //        var mergeItem = mergeList[i];

        //        var currentItem = baseCollection.SingleOrDefault(item => compare == null ? item.Equals(mergeItem) : compare(item, mergeItem));

        //        if (currentItem == null && onMerge != null)
        //        {
        //            currentItem = new T();
        //            if (onMerge != null)
        //                onMerge(currentItem, mergeItem);

        //            baseCollection.Add(currentItem);
        //            if (onAdd != null)
        //                onAdd(currentItem);
        //        }
        //    }

        //    return baseCollection;
        //}

        //static public T[] AddRequired<T>(this T[] array, T item)
        //{
        //    var list = array == null || array.Length <= 0 ? new List<T>() : array.ToList();
        //    list.Add(item);
        //    return list.ToArray();
        //}
    }
}
