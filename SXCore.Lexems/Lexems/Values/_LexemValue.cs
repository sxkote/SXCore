using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXLexemValue : SXLexem
    {
        public enum ValueType { None, Void, Bool, Date, Span, Number, Complex, Text, Struct };

        #region Properties
        public virtual ValueType Type
        { get { return ValueType.None; } }
        #endregion

        #region Functions
        public virtual SXLexemVariable Execute(SXLexem lexem, IEnvironment environment = null)
        {
            return null;
        }
        #endregion

        #region Operators
        public static implicit operator SXLexemValue(decimal value)
        { return (SXLexemNumber)value; }

        public static implicit operator SXLexemValue(double value)
        { return (SXLexemNumber)value; }

        public static implicit operator SXLexemValue(int value)
        { return (SXLexemNumber)value; }

        public static implicit operator SXLexemValue(bool value)
        { return (SXLexemBool)value; }

        public static implicit operator SXLexemValue(DateTime value)
        { return (SXLexemDate)value; }

        public static implicit operator SXLexemValue(TimeSpan value)
        { return (SXLexemSpan)value; }

        public static implicit operator SXLexemValue(string value)
        { return (SXLexemText)value; }
        #endregion

        #region Get Lexem
        public static SXLexemValue GetValue(SXLexem lexem)
        {
            if (lexem is SXLexemValue) 
                return (SXLexemValue)lexem;

            if (lexem is SXLexemVariable) 
                return ((SXLexemVariable)lexem).Value;
            
            return null;
        }

        new public static SXLexemValue Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            SXLexemValue result = null;
            if ((result = SXLexemVoid.Parse(ref text)) != null)
                return result;
            if ((result = SXLexemBool.Parse(ref text)) != null)
                return result;
            if ((result = SXLexemDate.Parse(ref text)) != null)
                return result;
            if ((result = SXLexemSpan.Parse(ref text)) != null)
                return result;
            if ((result = SXLexemText.Parse(ref text)) != null)
                return result;
            if ((result = SXLexemNumber.Parse(ref text)) != null)
                return result;
            if ((result = SXLexemComplex.Parse(ref text)) != null)
                return result;
            if ((result = SXLexemStruct.Parse(ref text)) != null)
                return result;
            return null;
        }

        public static SXLexemValue ParseExact(string text)
        {
            var temp = text;

            var result = SXLexemValue.Parse(ref temp);
            if (String.IsNullOrEmpty(temp))
                return result;

            return null;
        }
        #endregion
    }
}
