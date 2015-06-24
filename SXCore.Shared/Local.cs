using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;

namespace SXCore.Shared
{
    public class SXLocal
    {
        #region variables
        protected static string new_line = "\r\n";

        protected static string settings_file_name = "\\sx_settings.sxxml";

        protected static char[] hex_digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        #endregion

        #region RegionalSettings
        public static char[] HexDigits
        { get { return SXLocal.hex_digits; } }

        public static string NumberDecimalSeparator
        { get { return System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator; } }

        public static string CurrencyDecimalSeparator
        { get { return System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator; } }

        public static string PercentDecimalSeparator
        { get { return System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentDecimalSeparator; } }

        public static string ListSeparator
        { get { return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator; } }

        public static string NewLine
        {
            get { return SXLocal.new_line; }
            set { SXLocal.new_line = value; }
        }

        public static string SettingsFileName
        {
            get { return SXLocal.settings_file_name; }
            set { SXLocal.settings_file_name = value; }
        }
        #endregion

        #region Convertations
        #region Values
        public static decimal ConvertToDecimal(object obj)
        { return ((obj == null) ? 0 : Convert.ToDecimal(obj.ToString().Replace(",", SXLocal.NumberDecimalSeparator).Replace(".", SXLocal.NumberDecimalSeparator))); }

        public static bool ConvertToDecimal(object obj, out decimal value)
        {
            value = 0;

            try
            {
                value = SXLocal.ConvertToDecimal(obj);
                return true;
            }
            catch { return false; }
        }

        public static double ConvertToDouble(object obj)
        { return ((obj == null) ? 0 : Convert.ToDouble(obj.ToString().Replace(",", SXLocal.NumberDecimalSeparator).Replace(".", SXLocal.NumberDecimalSeparator))); }

        public static bool ConvertToDouble(object obj, out double value)
        {
            value = 0;

            try
            {
                value = SXLocal.ConvertToDouble(obj);
                return true;
            }
            catch { return false; }
        }

        public static int ConvertToInt(object obj)
        { return (obj == null) ? 0 : Convert.ToInt32(obj); }

        public static bool ConvertToInt(object obj, out int value)
        {
            value = 0;
            try
            {
                value = SXLocal.ConvertToInt(obj);
                return true;
            }
            catch { return false; }
        }

        public static bool StringContains(string str, string argument)
        {
            return (str.IndexOf(argument) >= 0);
        }
        #endregion

        #region Hexes
        public static int ConvertHexChar(char ch)
        {
            for (int i = 0; i < SXLocal.HexDigits.Length; i++)
                if (SXLocal.HexDigits[i] == ch)
                    return i;
            return -1;
        }

        public static int ConvertHexChar(string chars)
        {
            string str = chars.Replace(" ", "");
            int result = 0;
            for (int i = 0; i < str.Length; i++)
                result += (int)Math.Pow(16, i) * SXLocal.ConvertHexChar(str[str.Length - 1 - i]);
            return result;
        }

        public static string ConvertHex(byte b)
        {
            int val = Convert.ToInt32(b);
            int c1 = val / 16;
            int c2 = val - c1 * 16;
            return SXLocal.HexDigits[c1].ToString() + SXLocal.HexDigits[c2].ToString();
        }

        public static string ConvertHex(int val)
        {
            string result = "";
            int cur_val = val;

            if (val == 0) return "00";

            while (cur_val > 0)
            {
                int cur_res = cur_val % 16;
                cur_val = (cur_val - cur_res) / 16;
                result = SXLocal.HexDigits[cur_res].ToString() + result;
            }
            //result = SXLocalEnvironment.HexDigits[cur_val].ToString() + result;

            while (result.Length % 2 != 0)
                result = "0" + result;

            result = result.Replace(" ", "");
            string full_result = "";
            for (int i = 0; i < result.Length; i++)
            {
                full_result += result[i].ToString();
                if (full_result.Length > 1 && (full_result.Length - 2) % 3 == 0)
                    full_result += " ";
            }

            return full_result.Trim();
        }

        public static string ConvertHex(byte[] array)
        {
            string result = "";
            for (int i = 0; i < array.Length; i++)
                result += SXLocal.ConvertHex(array[i]) + " ";
            return result;
        }

        public static byte[] ConvertHexString(string str)
        {
            string[] byte_array = str.Split(new char[1] { ' ' });
            byte[] result = new byte[byte_array.Length];
            for (int i = 0; i < byte_array.Length; i++)
            {
                if (byte_array[i].Trim() == "") continue;

                string cur_val = byte_array[i].Trim();
                if (cur_val.Length != 2)
                    return null;
                int cur_int = SXLocal.ConvertHexChar(cur_val[0]) * 16 + SXLocal.ConvertHexChar(cur_val[1]);
                result[i] = Convert.ToByte(cur_int);
            }
            return result;
        }

        public static string ConvertBase64Array(byte[] array)
        { return Convert.ToBase64String(array); }

        public static byte[] ConvertBase64String(string str)
        { return Convert.FromBase64String(str); }
        #endregion

        #region ByteString
        public static byte[] ConvertByteString(Encoding encoding, string str)
        { return encoding.GetBytes(str); }

        public static string ConvertByteString(Encoding encoding, byte[] arr)
        { return encoding.GetString(arr, 0, arr.Length); }
        #endregion
        #endregion

        #region Files And Folders
        public static bool CreatePath(string path)
        {
            string cur_folder = (path[path.Length - 1] == Path.DirectorySeparatorChar) ? path.Substring(0, path.Length - 1) : path;
            if (!Directory.Exists(cur_folder))
            {
                string parent_folder = (new DirectoryInfo(cur_folder)).Parent.FullName;
                if (!Directory.Exists(parent_folder))
                    SXLocal.CreatePath(parent_folder);
                try { Directory.CreateDirectory(cur_folder); }
                catch { return false; }
            }
            return true;
        }
        #endregion
    }
}
