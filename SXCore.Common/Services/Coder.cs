using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Services
{
    public class Coder
    {
        protected static char[] Digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        public const int MinRadixLength = 2;
        public const int BinRadixLength = 2;
        public const int OctRadixLength = 8;
        public const int DecRadixLength = 10;
        public const int HexRadixLength = 16;
        public const int MaxRadixLength = 36;

        private readonly int _radix = 16;

        public int Radix
        {
            get
            {
                if (_radix < MinRadixLength)
                    return MinRadixLength;
                if (_radix > MaxRadixLength)
                    return MaxRadixLength;
                return _radix;
            }
        }

        public Coder(int radix = 16)
        {
            if (radix > MaxRadixLength)
                _radix = MaxRadixLength;
            else if (radix < MinRadixLength)
                _radix = MinRadixLength;
            else 
                _radix = radix;
        }

        public string Encode(long value)
        {
            string result = "";

            long rest = value;

            var length = this.Radix;

            while (rest > length)
            {
                var letter = rest % this.Radix;
                result = Coder.Digits[letter] + result;
                rest = rest / this.Radix;
            }

            return Coder.Digits[rest % this.Radix] + result;
        }

        public string Encode(DateTime date)
        {
            return String.Format("{0}{1}{2}{3}{4}",
                                this.Encode(date.Year - 1900).PadLeft(2, '0'),
                                this.Encode(date.Month),
                                this.Encode(date.Day),
                                this.Encode(date.Hour),
                                this.Encode(date.Minute).PadLeft(2, '0'));
        }

        public long Decode(string value)
        {
            if (String.IsNullOrEmpty(value))
                return 0;

            long result = 0;
            for (int i = 0; i < value.Length; i++)
                result += Digits.ToList().IndexOf(value[value.Length - 1 - i]) * (long)Math.Pow(this.Radix, i);

            return result;
        }

        public string Generate(int codeLength)
        {
            var rand = CommonService.Randomizer;

            string result = "";
            for (int i = 0; i < Math.Max(codeLength, 0); i++)
                result += Digits[rand.Next(this.Radix)];
            return result;
        }

        static public string Generate(int codeLength, int radix = MaxRadixLength, bool capitalize = false)
        {
            var coder = new Coder(radix);
            var code = coder.Generate(codeLength).ToLower();

            if (capitalize && code.Length > 1)
            {
                var rand = CommonService.Randomizer;
                int capitalCount = rand.Next(1, code.Length);

                char[] chars = code.ToCharArray();
                for (int i = 0; i < capitalCount; i++)
                {
                    int index = rand.Next(0, chars.Length);
                    chars[index] = Char.ToUpper(chars[index]);
                }

                code = new String(chars);
            }

            return code;
        }
    }
}
