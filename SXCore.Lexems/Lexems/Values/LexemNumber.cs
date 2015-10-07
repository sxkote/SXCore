using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXLexemNumber : SXLexemValue
    {
        public const string InfinityText = "infinity";

        #region Variables
        protected bool _infinity = false;
        protected double _value = 0;
        #endregion

        #region Properties
        public override ValueType Type
        { get { return ValueType.Number; } }

        public bool IsInfinity
        { get { return _infinity; } }

        public double Value
        { get { return _value; } }
        #endregion

        #region Constructors
        public SXLexemNumber(string text, bool signed = false)
        {
            string input = String.IsNullOrEmpty(text) ? null : text.Trim();
            if (String.IsNullOrEmpty(input))
                throw new FormatException("Number Value input can't be empty");

            bool isNegative = false;
            if (signed && (input[0] == '-' || input[0] == '+'))
            {
                isNegative = input[0] == '-';
                input = input.Substring(1);
            }

            if (input.ToLower() == InfinityText)
            {
                _infinity = true;
                _value = 1;
            }
            else
            {
                _infinity = false;
                _value = ParseDouble(input, false);
            }

            if (isNegative)
                _value *= -1;
        }

        public SXLexemNumber(double value)
        {
            _infinity = false;
            _value = value;
        }
        #endregion

        #region Common
        public override string ToString()
        {
            if (this.IsInfinity)
                return String.Format("{0}{1}", this.Value < 0 ? "-" : "", InfinityText);

            return this.Value.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is SXLexemNumber) 
                return ((SXLexemNumber)obj).IsInfinity == this.IsInfinity && ((SXLexemNumber)obj).Value == this.Value;

            if (this.IsInfinity)
                return false;

            if (obj is decimal) return (decimal)this.Value == (decimal)obj;
            if (obj is double) return (double)this.Value == (double)obj;
            if (obj is float) return (float)this.Value == (float)obj;
            if (obj is int) return (int)this.Value == (int)obj;

            return false;
        }

        public override int GetHashCode()
        { return this.Value.GetHashCode(); }
        #endregion

        #region Calculations
        public override SXLexemVariable Execute(SXLexem lexem, IEnvironment environment = null)
        {
            if (lexem == null) return null;

            if (lexem is SXLexemFunction)
            {
                var func = lexem as SXLexemFunction;

                switch (((SXLexemFunction)lexem).Name.ToLower())
                {
                    case "tostring":
                        return this.Value.ToString();
                }
            }

            return base.Execute(lexem, environment);
        }
        #endregion

        #region Operators
        public static implicit operator SXLexemNumber(decimal number)
        { return new SXLexemNumber((double)number); }

        public static implicit operator SXLexemNumber(double number)
        { return new SXLexemNumber(number); }

        public static implicit operator SXLexemNumber(int number)
        { return new SXLexemNumber((double)number); }

        public static SXLexemNumber operator +(SXLexemNumber argument, SXLexemNumber number)
        {
            var result = argument.Value + number.Value;

            if (!argument.IsInfinity && !number.IsInfinity)
                return result;

            if (argument.IsInfinity && !number.IsInfinity)
            {
                return argument;
            }

            if (!argument.IsInfinity && number.IsInfinity)
            {
                return number;
            }

            if (argument.IsInfinity && number.IsInfinity)
            {
                if (result == 0)
                    return 0;

                return SXLexemNumber.Infinity(result > 0);
            }

            return null;
        }

        public static SXLexemNumber operator -(SXLexemNumber argument, SXLexemNumber number)
        {
            var result = argument.Value - number.Value;

            if (!argument.IsInfinity && !number.IsInfinity)
                return result;

            if (argument.IsInfinity && !number.IsInfinity)
            {
                return argument;
            }

            if (!argument.IsInfinity && number.IsInfinity)
            {
                return number * -1;
            }

            if (argument.IsInfinity && number.IsInfinity)
            {
                if (result == 0)
                    return 0;

                return SXLexemNumber.Infinity(result > 0);
            }

            return null;
        }

        public static SXLexemNumber operator *(SXLexemNumber argument, SXLexemNumber number)
        {
            var result = argument.Value * number.Value;

            if (!argument.IsInfinity && !number.IsInfinity)
                return result;

            if (argument.IsInfinity || number.IsInfinity)
            {
                if (result == 0)
                    return 0;

                return SXLexemNumber.Infinity(result > 0);
            }

            return null;
        }

        public static SXLexemNumber operator /(SXLexemNumber argument, SXLexemNumber number)
        {
            var result = (number.Value == 0) ? 0 : argument.Value / number.Value;

            if (!argument.IsInfinity && !number.IsInfinity)
            {
                if (number.Value == 0)
                {
                    if (argument.Value == 0)
                        return 0;

                    return SXLexemNumber.Infinity(argument.Value >= 0);
                }

                return result;
            }

            if (argument.IsInfinity && !number.IsInfinity)
            {
                return SXLexemNumber.Infinity(result >= 0);
            }

            if (!argument.IsInfinity && number.IsInfinity)
            {
                return 0;
            }

            if (argument.IsInfinity && number.IsInfinity)
            {
                return (result >= 0) ? 1 : -1;
            }

            return null;
        }

        public static bool operator ==(SXLexemNumber argument, SXLexemNumber number)
        {
            if (((object)argument) == null || ((object)number) == null)
                return false;

            if (argument.IsInfinity != number.IsInfinity)
                return false;

            if (argument.IsInfinity)
                return argument.Value * number.Value > 0;

            return argument.Value == number.Value;
        }

        public static bool operator !=(SXLexemNumber argument, SXLexemNumber number)
        {
            if (((object)argument) == null || ((object)number) == null)
                return true;

            if (argument.IsInfinity != number.IsInfinity)
                return true;

            if (argument.IsInfinity)
                return argument.Value * number.Value <= 0;

            return argument.Value != number.Value;
        }

        public static bool operator >(SXLexemNumber argument, SXLexemNumber number)
        {
            if (argument == null || number == null)
                return false;

            if (argument.IsInfinity && number.IsInfinity)
                return argument.Value > number.Value;

            if (argument.IsInfinity && !number.IsInfinity)
                return argument.Value > 0;

            if (!argument.IsInfinity && number.IsInfinity)
                return number.Value < 0;

            if (!argument.IsInfinity && !number.IsInfinity)
                return argument.Value > number.Value;

            return false;
        }

        public static bool operator <(SXLexemNumber argument, SXLexemNumber number)
        { return number > argument; }

        public static bool operator >=(SXLexemNumber argument, SXLexemNumber number)
        {
            if (argument == null || number == null)
                return false;

            if (argument.IsInfinity && number.IsInfinity)
                return argument.Value >= number.Value;

            if (argument.IsInfinity && !number.IsInfinity)
                return argument.Value > 0;

            if (!argument.IsInfinity && number.IsInfinity)
                return number.Value < 0;

            if (!argument.IsInfinity && !number.IsInfinity)
                return argument.Value >= number.Value;

            return false;
        }

        public static bool operator <=(SXLexemNumber argument, SXLexemNumber number)
        { return number >= argument; }
        #endregion

        #region Statics
        static public SXLexemNumber Zero
        { get { return new SXLexemNumber(0); } }

        static public SXLexemNumber Infinity(bool positive = true)
        {
            return new SXLexemNumber(positive ? 1 : -1) { _infinity = true };
        }

        static public double ParseDouble(string input, bool signed = false)
        {
            double a = 0, b = 0;
            double decimalPart = 1;
            bool point = false;

            int start = 0;
            if (signed && (input[0] == '-' || input[0] == '+'))
                start = 1;

            for (int i = start; i < input.Length; i++)
            {
                if (input[i] == '.' || input[i] == ',')
                {
                    if (point)
                        throw new FormatException("Number Value can't contain multiple decimal separators");
                    else
                        point = true;
                }
                else if (Char.IsDigit(input[i]))
                {
                    if (point)
                        b += (input[i] - '0') * (decimalPart = decimalPart / 10);
                    else
                        a = a * 10 + (input[i] - '0');
                }
                else if (input[i] == 'E' || input[i] == 'e')
                {
                    var multiplicator = Math.Abs(Math.Pow(10, ParseDouble(input.Substring(i + 1), true)));

                    a *= multiplicator;
                    b *= multiplicator;

                    break;
                }
                else
                    throw new FormatException("Unexpected symbol in Number Value");
            }

            return (signed && input[0] == '-') ? -(a+b) : (a+b);
        }

        new static public SXLexemNumber Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            if (text.StartsWith(InfinityText, StringComparison.InvariantCultureIgnoreCase))
            {
                text = text.Crop(8);

                return SXLexemNumber.Infinity();
            }

            try
            {
                string input = "";

                #region get Number string value
                bool point = false, exponent = false;
                for (int i = 0; i < text.Length; i++)
                {
                    if (Char.IsDigit(text[i]))
                        input += text[i].ToString();
                    else if (text[i] == '.' || text[i] == ',')
                    {
                        if (point) return null;

                        input += text[i].ToString();
                        point = true;
                    }
                    else if (text[i].ToString().ToLower() == "e")
                    {
                        if (exponent) return null;

                        input += text[i].ToString();
                        exponent = true;
                        point = false;

                        if (i + 1 < text.Length && (text[i + 1] == '+' || text[i + 1] == '-'))
                        {
                            input += text[i + 1].ToString();
                            i++;
                        }
                    }
                    else
                        break;
                }
                #endregion

                if (String.IsNullOrEmpty(input) || input.Length == 1 && (input[0] == '.' || input[0] == ','))
                    return null;

                var result = new SXLexemNumber(input);

                text = text.Crop(input.Length);

                return result;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
