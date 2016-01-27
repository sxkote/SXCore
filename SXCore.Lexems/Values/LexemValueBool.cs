using System;

namespace SXCore.Lexems.Values
{
    public class LexemValueBool : LexemValue
    { 
        public bool Value { get; private set; }

        public LexemValueBool(string text)
        {
            if (String.IsNullOrEmpty(text))
                throw new ArgumentException("Boolean input can't be null or empty");

            if (text.Trim().Equals("true", StringComparison.InvariantCultureIgnoreCase))
                this.Value = true;
            else if (text.Trim().Equals("false", StringComparison.InvariantCultureIgnoreCase))
                this.Value = false;
            else
                throw new FormatException("Boolean format is incorrect");
        }

        public LexemValueBool(bool value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return this.Value ? "true" : "false";
        }

        public override bool Equals(object obj)
        {
            if (obj is LexemValueBool)
                return this.Value == ((LexemValueBool)obj).Value;

            if (obj is bool)
                return this.Value == (bool)obj;

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

        new public static LexemValueBool Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            if (text.StartsWith("true", StringComparison.InvariantCultureIgnoreCase))
            {
                text = text.Crop(4);
                return new LexemValueBool(true);
            }
            else if (text.StartsWith("false", StringComparison.InvariantCultureIgnoreCase))
            {
                text = text.Crop(5);
                return new LexemValueBool(false);
            }

            return null;
        }

        #region Operators
        public static implicit operator LexemValueBool(bool argument)
        {
            return new LexemValueBool(argument);
        }

        public static bool operator true(LexemValueBool argument)
        { return argument.Value; }

        public static bool operator false(LexemValueBool argument)
        { return argument.Value == false; }

        public static LexemValueBool operator |(LexemValueBool argument, LexemValueBool boolean)
        {
            return (argument.Value || boolean.Value);
        }

        public static LexemValueBool operator &(LexemValueBool argument, LexemValueBool boolean)
        {
            return (argument.Value && boolean.Value);
        }
        #endregion
    }
}
