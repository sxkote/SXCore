using System;
using System.Linq;

namespace SXCore.Common.Services
{
    public class CoderService
    {
        protected static char[] Digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        public const int MinBaseLength = 2;
        public const int OctBaseLength = 8;
        public const int HexBaseLength = 16;
        public const int MaxBaseLength = 36;

        private readonly int _baseLength;

        public int BaseLength
        {
            get
            {
                if (_baseLength < MinBaseLength)
                    return MinBaseLength;
                if (_baseLength > MaxBaseLength)
                    return MaxBaseLength;
                return _baseLength;
            }
        }

        public CoderService(int baseLength = HexBaseLength)
        {
            if (baseLength > MaxBaseLength)
                _baseLength = MaxBaseLength;
            else if (baseLength < MinBaseLength)
                _baseLength = MinBaseLength;
            else
                _baseLength = baseLength;
        }

        public string Encode(long value)
        {
            string result = "";

            long rest = value;

            var length = this.BaseLength;

            while (rest > length)
            {
                var letter = rest % this.BaseLength;
                result = CoderService.Digits[letter] + result;
                rest = rest / this.BaseLength;
            }

            return CoderService.Digits[rest % this.BaseLength] + result;
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

        public long Decode(string val)
        {
            if (String.IsNullOrEmpty(val))
                return 0;

            long result = 0;
            for (int i = 0; i < val.Length; i++)
                result += Digits.ToList().IndexOf(val[val.Length - 1 - i]) * (long)Math.Pow(this.BaseLength, i);

            return result;
        }

        public string Generate(int codeLength)
        {
            var rand = CommonService.Randomizer;

            string result = "";
            for (int i = 0; i < Math.Max(codeLength, 0); i++)
                result += Digits[rand.Next(this.BaseLength)];
            return result;
        }

        static public string GenerateCode(int codeLength, int baseLength = MaxBaseLength, bool capitalize = false)
        {
            var coder = new CoderService(baseLength);
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
