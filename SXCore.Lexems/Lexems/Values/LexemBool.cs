using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXLexemBool : SXLexemValue
    {
        public enum BoolType { Unknown, True, False };

        #region Variables
        protected BoolType _value = BoolType.Unknown;
        #endregion

        #region Properties
        public override ValueType Type
        { get { return ValueType.Bool; } }

        public BoolType Value
        { get { return _value; } }

        public SXLexemBool Negative
        {
            get
            {
                switch (this.Value)
                {
                    case BoolType.False:
                        return new SXLexemBool(BoolType.True);
                    case BoolType.True:
                        return new SXLexemBool(BoolType.False);
                    default: 
                        return new SXLexemBool(BoolType.Unknown);
                }
            }
        }
        #endregion

        #region Constructors
        public SXLexemBool(string text)
        {
            var input = text.Trim().ToLower();
            if (input == "true")
                _value = BoolType.True;
            else if (input == "false")
                _value = BoolType.False;
            else if (input == "unknown")
                _value = BoolType.Unknown;
            else
                throw new FormatException("Unknown Bool Value input");
        }

        public SXLexemBool(BoolType value)
        {
            _value = value;
        }
        #endregion

        #region Common
        public override string ToString()
        {
            switch (this.Value)
            {
                case BoolType.False:
                    return "false";
                case BoolType.True:
                    return "true";
                default:
                    return "unknown";
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is SXLexemBool)
                return this.Value == ((SXLexemBool)obj).Value;

            if (obj is bool)
            {
                if (((bool)obj) == true && this.Value == BoolType.True)
                    return true;

                if (((bool)obj) == false && this.Value == BoolType.False)
                    return true;
            }

            return false;
        }

        public override int GetHashCode()
        { return this.Value.GetHashCode(); }
        #endregion

        #region Operators
        public static implicit operator SXLexemBool(bool argument)
        {
            return new SXLexemBool((argument) ? BoolType.True : BoolType.False);
        }

        public static bool operator true(SXLexemBool argument)
        { return argument.Value == BoolType.True; }

        public static bool operator false(SXLexemBool argument)
        { return argument.Value == BoolType.False; }

        public static SXLexemBool operator |(SXLexemBool argument, SXLexemBool boolean)
        {
            if (argument.Value == BoolType.True || boolean.Value == BoolType.True)
                return true;

            if (argument.Value == BoolType.False && boolean.Value == BoolType.False)
                return false;

            return new SXLexemBool(SXLexemBool.BoolType.Unknown);
        }

        public static SXLexemBool operator &(SXLexemBool argument, SXLexemBool boolean)
        {
            if (argument.Value == BoolType.False || boolean.Value == BoolType.False)
                return false;

            if (argument.Value == BoolType.True && boolean.Value == BoolType.True)
                return true;

            return new SXLexemBool(SXLexemBool.BoolType.Unknown);
        }
        #endregion

        #region Statics
        new public static SXLexemBool Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            SXLexemBool result = null;
            if (text.StartsWith("true", StringComparison.InvariantCultureIgnoreCase))
                result = new SXLexemBool(BoolType.True);
            else if (text.StartsWith("false", StringComparison.InvariantCultureIgnoreCase))
                result = new SXLexemBool(BoolType.False);
            else if (text.StartsWith("unknown", StringComparison.InvariantCultureIgnoreCase))
                result = new SXLexemBool(BoolType.Unknown);

            if (result != null)
                text = text.Crop(result.ToString().Length);

            return result;
        }
        #endregion
    }
}
