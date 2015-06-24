using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    static internal class Extensions
    {
        static public string Crop(this string text, int length)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            if (text.Length <= length)
                return null;

            return text.Substring(length);
        }

        static public bool Like(this string text, string pattern)
        {
            pattern = System.Text.RegularExpressions.Regex.Escape(pattern);

            pattern = pattern.Replace("%", ".*?").Replace("_", ".");
            pattern = pattern.Replace(@"\[", "[").Replace(@"\]", "]").Replace(@"\^", "^");

            return System.Text.RegularExpressions.Regex.IsMatch(text, pattern);
        }
    }
}
