using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXLexemSpan : SXLexemValue
    {
        static public string[] Formats = { @"%d\.%h\:%m\:%s", @"%d %h\:%m\:%s", @"%d\;%h\;%m\;%s" };

        #region Variables
        protected TimeSpan _value = new TimeSpan(0, 0, 0, 0);
        #endregion

        #region Properties
        public override ValueType Type
        { get { return ValueType.Span; } }

        public TimeSpan Value
        { get { return _value; } }
        #endregion

        #region SInitializing
        public SXLexemSpan(string text)
        {
            var input = String.IsNullOrEmpty(text) ? null : text.Trim().ToLower();

            if (String.IsNullOrEmpty(input) || input.Length < 2 || input[0] != '\'' || input[input.Length - 1] != '\'')
                throw new FormatException("Span Value input can't be empty");

            input = input.Substring(1, input.Length - 2).Trim();
            if (String.IsNullOrEmpty(input))
                throw new FormatException("Span Value input can't be empty");

            if (!TimeSpan.TryParseExact(input, SXLexemSpan.Formats, CultureInfo.InvariantCulture, out _value))
                throw new FormatException("Span Value input has wrong format");
        }

        public SXLexemSpan(TimeSpan span)
        {
            _value = span;
        }
        #endregion

        #region Common
        public override string ToString()
        { return String.Format("'{0}'", this.Value.ToString(SXLexemSpan.Formats.FirstOrDefault())); }

        public override bool Equals(object obj)
        {
            if (obj is SXLexemSpan)
                return this.Value == ((SXLexemSpan)obj).Value;

            if (obj is TimeSpan)
                return this.Value == (TimeSpan)obj;

            return false;
        }

        public override int GetHashCode()
        { return this.Value.GetHashCode(); }
        #endregion

        #region Operators
        public static implicit operator SXLexemSpan(TimeSpan span)
        { return new SXLexemSpan(span); }

        public static implicit operator TimeSpan(SXLexemSpan span)
        { return span.Value; }

        public static bool operator ==(SXLexemSpan argument, SXLexemSpan span)
        {
            if (((object)argument) == null || ((object)span) == null)
                return false;

            return argument.Value == span.Value;
        }

        public static bool operator !=(SXLexemSpan argument, SXLexemSpan span)
        {
            if (((object)argument) == null || ((object)span) == null)
                return true;

            return argument.Value != span.Value;
        }

        public static bool operator >(SXLexemSpan argument, SXLexemSpan span)
        {
            if (argument == null || span == null)
                return false;

            return argument.Value > span.Value;
        }

        public static bool operator <(SXLexemSpan argument, SXLexemSpan span)
        { return span > argument; }

        public static bool operator >=(SXLexemSpan argument, SXLexemSpan span)
        {
            if (argument == null || span == null)
                return false;

            return argument.Value >= span.Value;
        }

        public static bool operator <=(SXLexemSpan argument, SXLexemSpan span)
        { return span >= argument; }
        #endregion

        #region Statics
        new public static SXLexemSpan Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            // trim the text and check possibility
            text = text.Trim();
            if (text.Length <= 2 || text[0] != '\'')
                return null;

            // find the end of the lexem
            var index = text.IndexOf('\'', 1);
            if (index <= 0)
                return null;

            // parse the span
            TimeSpan span;
            if (!TimeSpan.TryParseExact(text.Substring(1, index - 1), SXLexemSpan.Formats, null, out span))
                return null;

            // crop the lexem from the text
            text = text.Crop(index + 1);

            return new SXLexemSpan(span);
        }
        #endregion
    }
}
