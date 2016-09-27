using SXCore.Lexems.Values;
using System;

namespace SXCore.Lexems
{
    public enum LexemValueType { Pointer, Bool, Date, Span, Number, Complex, Text, Struct };

    public abstract class LexemValue : Lexem
    {
        public virtual LexemVariable Execute(Lexem lexem, ILexemEnvironment environment = null)
        {
            if (lexem == null)
                throw new InvalidOperationException("Can't execute null Lexem");

            return environment.Execute(this, lexem);
        }

        public static LexemValue GetValue(Lexem lexem)
        {
            if (lexem is LexemValue) 
                return (LexemValue)lexem;

            if (lexem is LexemVariable) 
                return ((LexemVariable)lexem).Value;
            
            return null;
        }

        new public static LexemValue Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            LexemValue result = null;
            if ((result = LexemValuePointer.Parse(ref text)) != null)
                return result;
            if ((result = LexemValueBool.Parse(ref text)) != null)
                return result;
            if ((result = LexemValueDate.Parse(ref text)) != null)
                return result;
            if ((result = LexemValueSpan.Parse(ref text)) != null)
                return result;
            if ((result = LexemValueText.Parse(ref text)) != null)
                return result;
            if ((result = LexemValueNumber.Parse(ref text)) != null)
                return result;
            if ((result = LexemValueComplex.Parse(ref text)) != null)
                return result;
            if ((result = LexemValueStruct.Parse(ref text)) != null)
                return result;
            return null;
        }

        public static LexemValue ParseExact(string text)
        {
            var temp = text;

            var result = LexemValue.Parse(ref temp);
            if (String.IsNullOrEmpty(temp))
                return result;

            return null;
        }

        #region Operators
        public static implicit operator LexemValue(decimal value)
        { return (LexemValueNumber)value; }

        public static implicit operator LexemValue(double value)
        { return (LexemValueNumber)value; }

        public static implicit operator LexemValue(int value)
        { return (LexemValueNumber)value; }

        public static implicit operator LexemValue(bool value)
        { return (LexemValueBool)value; }

        public static implicit operator LexemValue(DateTime value)
        { return (LexemValueDate)value; }

        public static implicit operator LexemValue(TimeSpan value)
        { return (LexemValueSpan)value; }

        public static implicit operator LexemValue(string value)
        { return (LexemValueText)value; }
        #endregion
    }
}
