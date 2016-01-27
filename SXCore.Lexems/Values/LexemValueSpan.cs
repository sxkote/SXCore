using System;
using System.Text.RegularExpressions;

namespace SXCore.Lexems.Values
{
    public class LexemValueSpan : LexemValue
    {
        public const string SpanPattern = @"\'(((?<days>\d+)[ \.\:])?(?<hours>\d+)\:(?<minutes>\d+)\:(?<seconds>\d+)|(?<days>\d*)\;(?<hours>\d*)\;(?<minutes>\d*)\;(?<seconds>\d*))\'";

        public TimeSpan Value { get; private set; }

        public LexemValueSpan(string text)
        {
            Match match = Regex.Match(text, $"^{SpanPattern}$");
            if (!match.Success)
                throw new FormatException("Span format is wrong");

            this.Value = CreateSpanFromRegexMatch(match);
        }

        public LexemValueSpan(TimeSpan span)
        {
            this.Value = span;
        }

        public override string ToString()
        { return $"'{this.Value.Days}:{this.Value.Hours}:{this.Value.Minutes}:{this.Value.Seconds}'"; }

        public override bool Equals(object obj)
        {
            if (obj is LexemValueSpan)
                return this.Value == ((LexemValueSpan)obj).Value;

            if (obj is TimeSpan)
                return this.Value == (TimeSpan)obj;

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

        static private TimeSpan CreateSpanFromRegexMatch(Match match)
        {
            // check the regex match
            if (match == null || !match.Success)
                throw new FormatException("Span has a wrong format");

            try
            {
                Func<string, int> getPart = name => String.IsNullOrEmpty(match.Groups[name].Value) ? 0 : Convert.ToInt32(match.Groups[name].Value);

                return new TimeSpan(getPart("days"), getPart("hours"), getPart("minutes"), getPart("seconds"));
            }
            catch
            {
                throw new FormatException("Number has a wrong format");
            }
        }

        new public static LexemValueSpan Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            Match match = Regex.Match(text, $"^{SpanPattern}");
            if (!match.Success)
                return null;

            text = text.Crop(match.Value.Length);

            return new LexemValueSpan(match.Value);
        }

        #region Operators
        public static implicit operator LexemValueSpan(TimeSpan span)
        { return new LexemValueSpan(span); }

        public static implicit operator TimeSpan(LexemValueSpan span)
        { return span.Value; }

        public static bool operator ==(LexemValueSpan argument, LexemValueSpan span)
        {
            if (ReferenceEquals(argument, null) || ReferenceEquals(span, null))
                return false;

            return argument.Value == span.Value;
        }

        public static bool operator !=(LexemValueSpan argument, LexemValueSpan span)
        { return !(argument == span); }

        public static bool operator >(LexemValueSpan argument, LexemValueSpan span)
        {
            if (argument == null || span == null)
                return false;

            return argument.Value > span.Value;
        }

        public static bool operator <(LexemValueSpan argument, LexemValueSpan span)
        { return span > argument; }

        public static bool operator >=(LexemValueSpan argument, LexemValueSpan span)
        {
            if (argument == null || span == null)
                return false;

            return argument.Value >= span.Value;
        }

        public static bool operator <=(LexemValueSpan argument, LexemValueSpan span)
        { return span >= argument; }
        #endregion
    }
}
