using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SXCore.Lexems.Values
{
    public class LexemValueNumber : LexemValue
    {
        public const string NumberPattern = @"(?<number>(\d+[\.\,]?\d*|[\.\,]\d+))(?<exp>[eE](?<expsign>[\-\+])?(?<expnum>\d+)?)?";

        static public LexemValueNumber Zero { get { return new LexemValueNumber(0); } }

        public double Value { get; private set; }

        public LexemValueNumber(string text, bool signed = false)
        { this.Value = ParseNumber(text, signed); }

        public LexemValueNumber(double value)
        { this.Value = value; }

        public override string ToString()
        { return this.Value.ToString(); }

        public override bool Equals(object obj)
        {
            if (obj is LexemValueNumber)
                return this.Value == ((LexemValueNumber)obj).Value;

            if (obj is decimal) return (decimal)this.Value == (decimal)obj;
            if (obj is double) return (double)this.Value == (double)obj;
            if (obj is float) return (float)this.Value == (float)obj;
            if (obj is int) return (int)this.Value == (int)obj;

            return false;
        }

        public override int GetHashCode()
        { return this.Value.GetHashCode(); }

        public override LexemVariable Execute(Lexem lexem, ILexemEnvironment environment = null)
        {
            if (lexem == null) return null;

            var function = lexem as LexemFunction;
            if (function != null)
            {
                switch (function.Name.ToLower())
                {
                    case "tostring":
                        return this.Value.ToString();
                }
            }

            return base.Execute(lexem, environment);
        }

        static private double CreateNumberFromRegexMatch(Match match)
        {
            // check the regex match
            if (match == null || !match.Success)
                throw new FormatException("Number has a wrong format");

            try
            {
                // getting the number value
                double result = Convert.ToDouble(match.Groups["number"].Value.Replace(",", "."), CultureInfo.InvariantCulture);

                // exponent apply
                if (match.Groups["exp"].Success)
                {
                    int expnum = match.Groups["expnum"].Success ? Convert.ToInt32(match.Groups["expnum"].Value) : 1;
                    if (match.Groups["expsign"].Success && match.Groups["expsign"].Value.Equals("-"))
                        expnum *= -1;

                    result *= Math.Abs(Math.Pow(10, expnum));
                }

                return result;
            }
            catch
            {
                throw new FormatException("Number has a wrong format");
            }
        }

        static public double ParseNumber(string text, bool signed = false)
        {
            //create a pattern string depends on the signed flag
            var pattern = String.Format("{0}{1}", signed ? @"(?<sign>[\-\+])?" : "", NumberPattern);

            Match match = Regex.Match(text?.Trim(), $"^{pattern}$");
            if (match == null || !match.Success || !signed && match.Groups["sign"].Success)
                throw new FormatException("Number has a wrong format");

            double result = CreateNumberFromRegexMatch(match);

            return (signed && match.Groups["sign"].Value == "-") ? -result : result;
        }

        new static public LexemValueNumber Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            Match match = Regex.Match(text, $"^{NumberPattern}");
            if (match == null || !match.Success)
                return null;

            text = text.Crop(match.Value.Length);

            return new LexemValueNumber(CreateNumberFromRegexMatch(match));
        }

        //static public double ParseNumber(string input, bool signed = false)
        //{
        //    double a = 0, b = 0;
        //    double decimalPart = 1;
        //    bool point = false;

        //    int start = 0;
        //    if (signed && (input[0] == '-' || input[0] == '+'))
        //        start = 1;

        //    for (int i = start; i < input.Length; i++)
        //    {
        //        if (input[i] == '.' || input[i] == ',')
        //        {
        //            if (point)
        //                throw new FormatException("Number can't contain multiple decimal separators");
        //            else
        //                point = true;
        //        }
        //        else if (Char.IsDigit(input[i]))
        //        {
        //            if (point)
        //                b += (input[i] - '0') * (decimalPart = decimalPart / 10);
        //            else
        //                a = a * 10 + (input[i] - '0');
        //        }
        //        else if (input[i] == 'E' || input[i] == 'e')
        //        {
        //            var multiplicator = Math.Abs(Math.Pow(10, ParseNumber(input.Substring(i + 1), true)));

        //            a *= multiplicator;
        //            b *= multiplicator;

        //            break;
        //        }
        //        else
        //            throw new FormatException("Unexpected symbol in Number Value");
        //    }

        //    return (signed && input[0] == '-') ? -(a + b) : (a + b);
        //}

        //new static public LexemNumber Parse(ref string text)
        //{
        //    if (String.IsNullOrEmpty(text))
        //        return null;

        //    text = text.Trim();

        //    try
        //    {
        //        string input = "";

        //        #region get Number string value
        //        bool point = false, exponent = false;
        //        for (int i = 0; i < text.Length; i++)
        //        {
        //            if (Char.IsDigit(text[i]))
        //                input += text[i].ToString();
        //            else if (text[i] == '.' || text[i] == ',')
        //            {
        //                if (point) return null;

        //                input += text[i].ToString();
        //                point = true;
        //            }
        //            else if (text[i].ToString().ToLower() == "e")
        //            {
        //                if (exponent) return null;

        //                input += text[i].ToString();
        //                exponent = true;
        //                point = false;

        //                if (i + 1 < text.Length && (text[i + 1] == '+' || text[i + 1] == '-'))
        //                {
        //                    input += text[i + 1].ToString();
        //                    i++;
        //                }
        //            }
        //            else
        //                break;
        //        }
        //        #endregion

        //        if (String.IsNullOrEmpty(input) || input.Length == 1 && (input[0] == '.' || input[0] == ','))
        //            return null;

        //        var result = new LexemNumber(input);

        //        text = text.Crop(input.Length);

        //        return result;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        #region Operators
        public static implicit operator LexemValueNumber(decimal number)
        { return new LexemValueNumber((double)number); }

        public static implicit operator LexemValueNumber(double number)
        { return new LexemValueNumber(number); }

        public static implicit operator LexemValueNumber(int number)
        { return new LexemValueNumber((double)number); }

        public static LexemValueNumber operator +(LexemValueNumber argument, LexemValueNumber number)
        {
            return argument.Value + number.Value;
        }

        public static LexemValueNumber operator -(LexemValueNumber argument, LexemValueNumber number)
        {
            return argument.Value - number.Value;
        }

        public static LexemValueNumber operator *(LexemValueNumber argument, LexemValueNumber number)
        {
            return argument.Value * number.Value;
        }

        public static LexemValueNumber operator /(LexemValueNumber argument, LexemValueNumber number)
        {
            return argument.Value / number.Value;
        }

        public static bool operator ==(LexemValueNumber argument, LexemValueNumber number)
        {
            if (ReferenceEquals(argument, null) || ReferenceEquals(number, null))
                return false;

            return argument.Value == number.Value;
        }

        public static bool operator !=(LexemValueNumber argument, LexemValueNumber number)
        {
            return !(argument == number);
        }

        public static bool operator >(LexemValueNumber argument, LexemValueNumber number)
        {
            if (argument == null || number == null)
                return false;

            return argument.Value > number.Value;
        }

        public static bool operator <(LexemValueNumber argument, LexemValueNumber number)
        { return number > argument; }

        public static bool operator >=(LexemValueNumber argument, LexemValueNumber number)
        {
            if (argument == null || number == null)
                return false;

            return argument.Value >= number.Value;
        }

        public static bool operator <=(LexemValueNumber argument, LexemValueNumber number)
        { return number >= argument; }
        #endregion
    }
}
