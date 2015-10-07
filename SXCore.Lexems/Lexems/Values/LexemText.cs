using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXLexemText : SXLexemValue
    {
        static public string[] Quotes = { "\"", "'" };

        #region Variables
        protected string _quote = "\"";
        protected string _value = "";
        #endregion

        #region Properties
        public override ValueType Type
        { get { return ValueType.Text; } }

        public string Quote
        { get { return _quote; } }

        public string Value
        { get { return _value; } }
        #endregion

        #region Constructors
        public SXLexemText(string value, string quote)
        {
            _value = value;
            _quote = quote;
        }
        #endregion

        #region Common
        public override string ToString()
        { return String.Format("{0}{1}{0}", this.Quote, SXLexemText.Encode(this.Value, this.Quote)); }

        public override bool Equals(object obj)
        {
            if (obj is SXLexemText)
                return this.Value == ((SXLexemText)obj).Value;

            return (obj is string && this.Value == obj.ToString());
        }

        public override int GetHashCode()
        { return this.Value.GetHashCode(); }
        #endregion

        #region Calculations
        public override SXLexemVariable Execute(SXLexem lexem, IEnvironment environment = null)
        {
            if (lexem == null) return null;

            if (lexem is SXLexemVariable)
            {
                switch (((SXLexemVariable)lexem).Name.Trim().ToLower())
                {
                    case "length":
                        return this.Value.Length;
                }
            }
            else if (lexem is SXLexemFunction)
            {
                SXLexemFunction func = lexem as SXLexemFunction;

                Func<int, string> getFuncString = i => (func.Arguments[i].Calculate(environment).Value as SXLexemText).Value;
                Func<int, double> getFuncNumber = i => (func.Arguments[i].Calculate(environment).Value as SXLexemNumber).Value;

                switch (func.Name.Trim().ToLower())
                {
                    case "tostring":
                        return this.Value;
                    case "tonumber":
                        return SXLexemNumber.ParseDouble(this.Value, true);
                    case "todatetime":
                    case "todate":
                        {
                            return SXLexemDate.ParseDatetime(this.Value);
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
                            if (func.Arguments.Count >= 2)
                                return this.Value.Substring((int)getFuncNumber(0), (int)getFuncNumber(1));
                            else
                                return this.Value.Substring((int)getFuncNumber(0));
                        }
                }
            }

            return base.Execute(lexem, environment);
        }
        #endregion

        #region Operators
        public static implicit operator SXLexemText(string text)
        {
            return new SXLexemText(text, SXLexemText.Quotes.FirstOrDefault());
        }

        public static implicit operator string(SXLexemText argument)
        {
            return argument.Value;
        }

        public static SXLexemText operator +(SXLexemText argument, SXLexemText text)
        {
            return new SXLexemText(argument.Value + text.Value, argument.Quote);
        }

        public static bool operator ==(SXLexemText argument, SXLexemText text)
        {
            if (((object)argument) == null || ((object)text) == null)
                return false;

            return argument.Value == text.Value;
        }

        public static bool operator !=(SXLexemText argument, SXLexemText text)
        {
            if (((object)argument) == null || ((object)text) == null)
                return true;

            return argument.Value != text.Value;
        }

        public static bool operator >(SXLexemText argument, SXLexemText text)
        {
            if (argument == null || text == null)
                return false;

            return argument.Value.Length > text.Value.Length;
        }

        public static bool operator <(SXLexemText argument, SXLexemText text)
        { return text > argument; }

        public static bool operator >=(SXLexemText argument, SXLexemText text)
        {
            if (argument == null || text == null)
                return false;

            return argument.Value.Length >= text.Value.Length;
        }

        public static bool operator <=(SXLexemText argument, SXLexemText text)
        { return text >= argument; }
        #endregion

        #region Statics
        public static string Encode(string text, string quote)
        {
            return text.Replace("&", "&amp;").Replace(quote, "&quote;");
        }

        public static string Decode(string text, string quote)
        {
            return text.Replace("&quote;", quote).Replace("&amp;", "&");
        }

        new public static SXLexemText Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            // get quote for the string 
            var quote = text[0].ToString();

            // check the input correctness
            if (text.Length < 2 || !SXLexemText.Quotes.Contains(quote))
                return null;

            // find the end of the lexem
            int index = text.IndexOf(quote, 1);
            if (index <= 0)
                return null;

            // get the value of the text lexem
            var value = text.Substring(1, index - 1);

            // crop the lexem from the text
            text = text.Crop(index + 1);

            return new SXLexemText(SXLexemText.Decode(value, quote), quote);
        }
        #endregion
    }
}
