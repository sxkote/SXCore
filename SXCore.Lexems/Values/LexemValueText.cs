using System;
using System.Linq;

namespace SXCore.Lexems.Values
{
    public class LexemValueText : LexemValue
    {
        static public string[] Quotes = { "\"", "'" };

        public string Quote { get; private set; }
        public string Value { get; private set; }
        
        public LexemValueText(string value, string quote)
        {
            this.Value = value;
            this.Quote = quote;
        }

        public override string ToString()
        { return String.Format("{0}{1}{0}", this.Quote, LexemValueText.Encode(this.Value, this.Quote)); }

        public override bool Equals(object obj)
        {
            if (obj is LexemValueText)
                return this.Value == ((LexemValueText)obj).Value;

            return (obj is string && this.Value == obj.ToString());
        }

        public override int GetHashCode()
        { return this.Value.GetHashCode(); }

        public override LexemVariable Execute(Lexem lexem, ILexemEnvironment environment = null)
        {
            if (lexem == null)
                throw new InvalidOperationException("Can't execute null lexem on Value");

            if (lexem is LexemVariable)
            {
                switch (((LexemVariable)lexem).Name.Trim().ToLower())
                {
                    case "length":
                        return this.Value.Length;
                }
            }
            else if (lexem is LexemFunction)
            {
                LexemFunction function = lexem as LexemFunction;

                Func<int, string> getFuncString = i => (function.Arguments[i].Calculate(environment).Value as LexemValueText).Value;
                Func<int, double> getFuncNumber = i => (function.Arguments[i].Calculate(environment).Value as LexemValueNumber).Value;

                switch (function.Name.Trim().ToLower())
                {
                    case "tostring":
                        return this.Value;
                    case "tonumber":
                        return LexemValueNumber.ParseNumber(this.Value, true);
                    case "todatetime":
                    case "todate":
                        {
                            return LexemValueDate.ParseDateTime(this.Value);
                        }
                    case "trim":
                        return this.Value.Trim();
                    case "tolower":
                        return this.Value.ToLower();
                    case "toupper":
                        return this.Value.ToUpper();
                    case "startswith":
                        return this.Value.StartsWith(getFuncString(0));
                    case "endswith":
                        return this.Value.EndsWith(getFuncString(0));
                    case "contains":
                        return this.Value.Contains(getFuncString(0));
                    case "like":
                        return this.Value.Like(getFuncString(0));
                    case "indexof":
                        return this.Value.IndexOf(getFuncString(0));
                    case "replace":
                        return this.Value.Replace(getFuncString(0), getFuncString(1));
                    case "substring":
                        {
                            if (function.Arguments.Count >= 2)
                                return this.Value.Substring((int)getFuncNumber(0), (int)getFuncNumber(1));
                            else
                                return this.Value.Substring((int)getFuncNumber(0));
                        }
                }
            }

            return base.Execute(lexem, environment);
        }

        public static string Encode(string text, string quote)
        {
            return text.Replace("&", "&amp;").Replace(quote, "&quote;");
        }

        public static string Decode(string text, string quote)
        {
            return text.Replace("&quote;", quote).Replace("&amp;", "&");
        }

        new public static LexemValueText Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            // get quote for the string 
            var quote = text[0].ToString();

            // check the input correctness
            if (text.Length < 2 || !LexemValueText.Quotes.Contains(quote))
                return null;

            // find the end of the lexem
            int index = text.IndexOf(quote, 1);
            if (index <= 0)
                return null;

            // get the value of the text lexem
            var value = text.Substring(1, index - 1);

            // crop the lexem from the text
            text = text.Crop(index + 1);

            return new LexemValueText(LexemValueText.Decode(value, quote), quote);
        }

        #region Operators
        public static implicit operator LexemValueText(string text)
        {
            return new LexemValueText(text, LexemValueText.Quotes.FirstOrDefault());
        }

        public static implicit operator string(LexemValueText argument)
        {
            return argument.Value;
        }

        public static LexemValueText operator +(LexemValueText argument, LexemValueText text)
        {
            return new LexemValueText(argument.Value + text.Value, argument.Quote);
        }

        public static bool operator ==(LexemValueText argument, LexemValueText text)
        {
            if (ReferenceEquals(argument, null) || ReferenceEquals(text, null))
                return false;

            return argument.Value == text.Value;
        }

        public static bool operator !=(LexemValueText argument, LexemValueText text)
        { return !(argument == text); }

        public static bool operator >(LexemValueText argument, LexemValueText text)
        {
            if (argument == null || text == null)
                return false;

            return argument.Value.Length > text.Value.Length;
        }

        public static bool operator <(LexemValueText argument, LexemValueText text)
        { return text > argument; }

        public static bool operator >=(LexemValueText argument, LexemValueText text)
        {
            if (argument == null || text == null)
                return false;

            return argument.Value.Length >= text.Value.Length;
        }

        public static bool operator <=(LexemValueText argument, LexemValueText text)
        { return text >= argument; }
        #endregion
    }
}
