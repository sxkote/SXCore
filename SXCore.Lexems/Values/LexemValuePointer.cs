using System;

namespace SXCore.Lexems.Values
{
    public class LexemValuePointer : LexemValue
    {
        public static LexemValuePointer Null
        { get { return new LexemValuePointer(null); } }

        public object Value { get; private set; }

        public LexemValuePointer(object pointer)
        {
            this.Value = pointer;
        }

        public override string ToString()
        {
            return (this.Value == null) ? "null" : this.Value.ToString();
        }

        new public static LexemValuePointer Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            if (!text.StartsWith("null", StringComparison.InvariantCultureIgnoreCase))
                return null;

            text = text.Crop(4);

            return LexemValuePointer.Null;
        }

        public override LexemVariable Execute(Lexem lexem, ILexemEnvironment environment = null)
        {
            return base.Execute(lexem, environment);
        }
    }
}
