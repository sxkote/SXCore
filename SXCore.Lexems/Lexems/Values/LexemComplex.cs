using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXLexemComplex : SXLexemValue
    {
        #region Variables
        protected bool _infinity = false;
        protected double _re = 0;
        protected double _im = 0;
        #endregion

        #region Properties
        public override ValueType Type
        { get { return ValueType.Complex; } }

        public bool IsInfinity
        { get { return _infinity; } }

        public double Re
        { get { return _re; } }

        public double Im
        { get { return _im; } }

        public double Norm
        { get { return Math.Sqrt(this.Re * this.Re + this.Im * this.Im); } }

        public SXLexemComplex Reversed
        { get { return new SXLexemComplex(this.Re, -this.Im); } }
        #endregion

        #region Constructor
        public SXLexemComplex(string text)
        {
            var input = String.IsNullOrEmpty(text) ? null : text.ToLower();

            if (String.IsNullOrEmpty(input) || input[0] != '[' || input[input.Length - 1] != ']')
                throw new FormatException("Complex Number Value has a wrong input");

            if (input == "[" + SXLexemNumber.InfinityText + "]")
            {
                _infinity = true;
                return;
            }

            var split = input.Substring(1, input.Length - 2).Split(new char[] { ';' });
            if (split == null || split.Length != 2)
                throw new FormatException("Complex Number Value has a wrong input");

            _re = SXLexemNumber.ParseDouble(split[0], true);
            _im = SXLexemNumber.ParseDouble(split[1], true);
        }

        public SXLexemComplex(double re, double im)
        {
            _re = re;
            _im = im;
        }
#endregion

        #region Common
        public override string ToString()
        {
            if (this.IsInfinity)
                return String.Format("[{0}]", SXLexemNumber.InfinityText);

            return String.Format("[{0};{1}]", this.Re, this.Im); 
        }

        public override bool Equals(object obj)
        {
            if (obj is SXLexemComplex)
            {
                var compl = obj as SXLexemComplex;

                if (compl.IsInfinity && this.IsInfinity)
                    return true;

                if (compl.IsInfinity || this.IsInfinity)
                    return false;

                return compl.Re == this.Re && compl.Im == this.Im;
            }

            if (this.IsInfinity || this.Im != 0)
                return false;

            if (obj is SXLexemNumber) 
                return (double)((SXLexemNumber)obj).Value == this.Re;

            if (obj is decimal || obj is double || obj is float || obj is int) 
                return this.Re == (double)obj;

            return false;
        }

        public override int GetHashCode()
        { return this.Norm.GetHashCode(); }
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
                        return this.ToString();
                }
            }

            return base.Execute(lexem, environment);
        }
        #endregion

        #region Operators
        public static implicit operator SXLexemComplex(double number)
        { return new SXLexemComplex(number, 0); }

        public static implicit operator SXLexemComplex(SXLexemNumber number)
        { return new SXLexemComplex((double)number.Value, 0); }

        public static SXLexemComplex operator +(SXLexemComplex argument, SXLexemComplex number)
        {
            if (argument.IsInfinity || number.IsInfinity)
                return SXLexemComplex.Infinity;

            return new SXLexemComplex(argument.Re + number.Re, argument.Im + number.Im);
        }

        public static SXLexemComplex operator -(SXLexemComplex argument, SXLexemComplex number)
        {
            if (argument.IsInfinity && number.IsInfinity)
                return SXLexemComplex.Zero;

            if (argument.IsInfinity || number.IsInfinity)
                return SXLexemComplex.Infinity;

            return new SXLexemComplex(argument.Re - number.Re, argument.Im - number.Im);
        }

        public static SXLexemComplex operator *(SXLexemComplex argument, SXLexemComplex number)
        {
            if (argument.IsInfinity || number.IsInfinity)
                return SXLexemComplex.Infinity;

            return new SXLexemComplex(argument.Re * number.Re - argument.Im * number.Im, argument.Re * number.Im + number.Re * argument.Im);
        }

        public static SXLexemComplex operator /(SXLexemComplex argument, SXLexemComplex number)
        {
            if (argument.IsInfinity)
            {
                if (number.IsInfinity)
                    return new SXLexemComplex(1, 0);
                else
                    return SXLexemComplex.Infinity;
            }

            if (number.IsInfinity)
                return SXLexemComplex.Zero;

            if (number.Norm == 0)
                return SXLexemComplex.Infinity;

            return new SXLexemComplex((argument.Re * number.Re + argument.Im * number.Im) / number.Norm, (argument.Im * number.Re - argument.Re * number.Im) / number.Norm);
        }

        public static bool operator ==(SXLexemComplex argument, SXLexemComplex number)
        {
            if (argument.IsInfinity != number.IsInfinity)
                return false;

            if (argument.IsInfinity == number.IsInfinity)
                return true;

            return argument.Re == number.Re && argument.Im == number.Im;
        }

        public static bool operator !=(SXLexemComplex argument, SXLexemComplex number)
        {
            if (argument.IsInfinity != number.IsInfinity)
                return true;

            if (argument.IsInfinity == number.IsInfinity)
                return false;

            return argument.Re != number.Re || argument.Im != number.Im;
        }

        public static bool operator >(SXLexemComplex argument, SXLexemComplex number)
        {
            if (number.IsInfinity)
                return false;

            if (argument.IsInfinity)
                return true;

            return argument.Norm > number.Norm;
        }

        public static bool operator <(SXLexemComplex argument, SXLexemComplex number)
        {
            return number > argument;
        }

        public static bool operator >=(SXLexemComplex argument, SXLexemComplex number)
        {
            if (number.IsInfinity && argument.IsInfinity)
                return true;

            if (number.IsInfinity)
                return false;

            if (argument.IsInfinity)
                return true;

            return argument.Norm > number.Norm;
        }

        public static bool operator <=(SXLexemComplex argument, SXLexemComplex number)
        {
            return number >= argument;
        }
        #endregion

        #region Statics
        static public SXLexemComplex Zero
        { get { return new SXLexemComplex(0, 0); } }

        static public SXLexemComplex Infinity
        { get { return new SXLexemComplex("[" + SXLexemNumber.InfinityText + "]"); } }

        new public static SXLexemComplex Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            if (text[0] != '[')
                return null;

            int index = SXLexem.Find(text, new char[] { ']' });
            if (index <= 0)
                return null;

            var input = text.Substring(0, index + 1);
            if (input.Trim().ToLower() != "[" + SXLexemNumber.InfinityText + "]" && input.Count(ch => ch == ';') != 1)
                return null;

            try
            {
                var result = new SXLexemComplex(input);

                text = text.Crop(index + 1);

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
