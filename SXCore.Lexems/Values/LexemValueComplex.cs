using System;
using System.Text.RegularExpressions;

namespace SXCore.Lexems.Values
{
    public class LexemValueComplex : LexemValue
    {
        public const string ComplexPattern = @"\[(?<complex>infinity|(?<re>[\-\+]?\d+([\.\,]\d*)?([eE][\-\+]?\d*)?)\;(?<im>[\-\+]?\d+([\.\,]\d*)?([eE][\-\+]?\d*)?))\]";

        static public LexemValueComplex Zero { get { return new LexemValueComplex(0, 0); } }
        static public LexemValueComplex Infinity { get { return new LexemValueComplex(0, 0) { IsInfinity = true }; } }

        public bool IsInfinity { get; private set; }
        public double Re { get; private set; }
        public double Im { get; private set; }

        public double Norm
        { get { return Math.Sqrt(this.Re * this.Re + this.Im * this.Im); } }

        public LexemValueComplex Reversed
        { get { return new LexemValueComplex(this.Re, -this.Im); } }

        public LexemValueComplex(string text)
        {
            Match match = Regex.Match(text, $"^{ComplexPattern}$");
            if (!match.Success)
                throw new FormatException("Complex Value has a wrong format");

            if (match.Groups["re"].Success && match.Groups["im"].Success)
            {
                this.IsInfinity = false;
                this.Re = LexemValueNumber.ParseNumber(match.Groups["re"].Value, true);
                this.Im = LexemValueNumber.ParseNumber(match.Groups["im"].Value, true);
            }
            else
            {
                this.IsInfinity = true;
                this.Re = 0;
                this.Im = 0;
            }
        }

        public LexemValueComplex(double re, double im)
        {
            this.IsInfinity = false;
            this.Re = re;
            this.Im = im;
        }

        public override string ToString()
        {
            return this.IsInfinity ? $"[infinity]" : $"[{this.Re};{this.Im}]";
        }

        public override bool Equals(object obj)
        {
            if (obj is LexemValueComplex)
            {
                var compl = obj as LexemValueComplex;

                if (compl.IsInfinity && this.IsInfinity)
                    return true;

                if (compl.IsInfinity || this.IsInfinity)
                    return false;

                return compl.Re == this.Re && compl.Im == this.Im;
            }

            if (this.IsInfinity || this.Im != 0)
                return false;

            if (obj is LexemValueNumber) 
                return (double)((LexemValueNumber)obj).Value == this.Re;

            if (obj is decimal || obj is double || obj is float || obj is int) 
                return this.Re == (double)obj;

            return false;
        }

        public override int GetHashCode()
        { return this.Norm.GetHashCode(); }

        public override LexemVariable Execute(Lexem lexem, ILexemEnvironment environment = null)
        {
            if (lexem == null)
                throw new InvalidOperationException("Can't execute null lexem on Value");

            var function = lexem as LexemFunction;
            if (function != null)
            {
                switch (function.Name.ToLower())
                {
                    case "tostring":
                        return this.ToString();
                }
            }

            return base.Execute(lexem, environment);
        }

        new public static LexemValueComplex Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            Match match = Regex.Match(text, $"^{ComplexPattern}");
            if (!match.Success)
                return null;

            text = text.Crop(match.Value.Length);

            return new LexemValueComplex(match.Value);
        }

        #region Operators
        public static implicit operator LexemValueComplex(double number)
        { return new LexemValueComplex(number, 0); }

        public static implicit operator LexemValueComplex(LexemValueNumber number)
        { return new LexemValueComplex((double)number.Value, 0); }

        public static LexemValueComplex operator +(LexemValueComplex argument, LexemValueComplex number)
        {
            if (argument.IsInfinity || number.IsInfinity)
                return LexemValueComplex.Infinity;

            return new LexemValueComplex(argument.Re + number.Re, argument.Im + number.Im);
        }

        public static LexemValueComplex operator -(LexemValueComplex argument, LexemValueComplex number)
        {
            if (argument.IsInfinity && number.IsInfinity)
                return LexemValueComplex.Zero;

            if (argument.IsInfinity || number.IsInfinity)
                return LexemValueComplex.Infinity;

            return new LexemValueComplex(argument.Re - number.Re, argument.Im - number.Im);
        }

        public static LexemValueComplex operator *(LexemValueComplex argument, LexemValueComplex number)
        {
            if (argument.IsInfinity || number.IsInfinity)
                return LexemValueComplex.Infinity;

            return new LexemValueComplex(argument.Re * number.Re - argument.Im * number.Im, argument.Re * number.Im + number.Re * argument.Im);
        }

        public static LexemValueComplex operator /(LexemValueComplex argument, LexemValueComplex number)
        {
            if (argument.IsInfinity)
            {
                if (number.IsInfinity)
                    return new LexemValueComplex(1, 0);
                else
                    return LexemValueComplex.Infinity;
            }

            if (number.IsInfinity)
                return LexemValueComplex.Zero;

            if (number.Norm == 0)
                return LexemValueComplex.Infinity;

            return new LexemValueComplex((argument.Re * number.Re + argument.Im * number.Im) / number.Norm, (argument.Im * number.Re - argument.Re * number.Im) / number.Norm);
        }

        public static bool operator ==(LexemValueComplex argument, LexemValueComplex number)
        {
            return argument.IsInfinity && number.IsInfinity || !argument.IsInfinity && !number.IsInfinity && argument.Re == number.Re && argument.Im == number.Im;
        }

        public static bool operator !=(LexemValueComplex argument, LexemValueComplex number)
        { return !(argument == number); }

        public static bool operator >(LexemValueComplex argument, LexemValueComplex number)
        {
            return !number.IsInfinity && (argument.IsInfinity || argument.Norm > number.Norm);
        }

        public static bool operator <(LexemValueComplex argument, LexemValueComplex number)
        {
            return number > argument;
        }

        public static bool operator >=(LexemValueComplex argument, LexemValueComplex number)
        {
            return argument.IsInfinity || !number.IsInfinity && argument.Norm >= number.Norm;
        }

        public static bool operator <=(LexemValueComplex argument, LexemValueComplex number)
        {
            return number >= argument;
        }
        #endregion
    }
}
