using SXCore.Common.Exceptions;
using SXCore.Common.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common
{
    public static class Extensions
    {
        public static byte[] ReadFully(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static async Task<byte[]> ReadFullyAsync(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            if (query != null && pageNumber > 0 && pageSize > 0)
                return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return query;
        }

        public static IEnumerable<T> Page<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
        {
            if (source != null && pageNumber > 0 && pageSize > 0)
                return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return source;
        }

        public static ParamValueCollection SplitParams(this string input, params char[] separators)
        {
            var result = new ParamValueCollection();

            if (String.IsNullOrWhiteSpace(input))
                return result;

            var items = SplitFormatted(input, separators);
            foreach (var item in items)
            {
                if (String.IsNullOrWhiteSpace(item))
                    continue;

                var split = item.Split('=');
                if (split == null || split.Length != 2)
                    throw new CustomArgumentException("Param's Name and Value should be separated by '='");

                var name = split[0].Trim();
                if (name.Length > 2 && name[0] == name[name.Length - 1] && separators.Contains(name[0]))
                    name = name.Substring(1, name.Length - 2);

                var value = split[1].Trim();
                if (value.Length > 2 && value[0] == value[name.Length - 1] && separators.Contains(value[0]))
                    value = value.Substring(1, value.Length - 2);

                result.Add(name, value);
            }

            return result;
        }

        public static ParamValueCollection SplitParams(this string input)
        { return SplitParams(input, ';'); }

        public static IEnumerable<string> SplitFormatted(this string input, params char[] separators)
        {
            var sb = new StringBuilder();
            bool quoted = false;
            bool apostrophed = false;

            foreach (char c in input)
            {
                if (quoted || apostrophed)
                {
                    if (quoted && c == '"')
                        quoted = false;
                    else if (apostrophed && c == '\'')
                        apostrophed = false;

                    sb.Append(c);
                }
                else
                {
                    if (separators.Contains(c))
                    {
                        yield return sb.ToString();
                        sb.Length = 0;
                        continue;
                    }

                    if (c == '"')
                        quoted = true;
                    else if (c == '\'')
                        apostrophed = true;

                    sb.Append(c);
                }
            }

            if (quoted || apostrophed)
                throw new CustomArgumentException("Unterminated quotation mark.");

            yield return sb.ToString();
        }
    }
}
