using System;
using System.Globalization;
using System.Linq;

namespace SXCore.Lexems.Values
{
    public class LexemValueDate : LexemValue
    {
        static public string[] Formats = { "dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy HH:mm", "dd.MM.yyyy", "dd.MM.yy HH:mm:ss", "dd.MM.yy HH:mm", "dd.MM.yy", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm" };

        public DateTime Value { get; private set; }

        public LexemValueDate(string text)
        {
            var input = text?.Trim();

            if (String.IsNullOrEmpty(input) || input.Length <= 2 || input[0] != '\'' || input[input.Length - 1] != '\'')
                throw new FormatException("DateTime format is incorrect");

            input = input.Substring(1, input.Length - 2).Trim();

            if (input.Equals("now", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Value = DateTime.Now;
            }
            else
            {
                DateTime value;
                if (!DateTime.TryParseExact(input, LexemValueDate.Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
                    throw new FormatException("DateTime format is incorrect");

                this.Value = value;
            }
        }

        public LexemValueDate(DateTime date)
        {
            this.Value = date;
        }

        public override string ToString()
        {
            return $"'{this.Value.ToString(Formats.First())}'";
        }

        public override bool Equals(object obj)
        {
            if (obj is LexemValueDate)
                return this.Value == ((LexemValueDate)obj).Value;

            if (obj is DateTime)
                return this.Value == (DateTime)obj;

            return false;
        }

        public override int GetHashCode()
        { return this.Value.GetHashCode(); }

        public override LexemVariable Execute(Lexem lexem, ILexemEnvironment environment = null)
        {
            if (lexem == null) return null;

            if (lexem is LexemVariable)
            {
                switch (((LexemVariable)lexem).Name.ToLower())
                {
                    case "date":
                        return this.Value.Date;
                    case "year":
                        return this.Value.Year;
                    case "month":
                        return this.Value.Month;
                    case "day":
                        return this.Value.Day;
                    case "hour":
                        return this.Value.Hour;
                    case "minute":
                        return this.Value.Minute;
                    case "second":
                        return this.Value.Second;
                    case "dayofweek":
                        return this.Value.DayOfWeek.ToString();
                }
            }
            else if (lexem is LexemFunction)
            {
                var function = lexem as LexemFunction;

                Func<int, double> getFuncNumber = i => (function.Arguments[i].Calculate(environment).Value as LexemValueNumber).Value;

                switch (((LexemFunction)lexem).Name.ToLower())
                {
                    case "tostring":
                        return this.Value.ToString(Formats.First());
                    case "addyears":
                        return this.Value.Date.AddYears((int)getFuncNumber(0));
                    case "addmonths":
                        return this.Value.Date.AddMonths((int)getFuncNumber(0));
                    case "adddays":
                        return this.Value.Date.AddDays(getFuncNumber(0));
                    case "addhours":
                        return this.Value.Date.AddHours(getFuncNumber(0));
                    case "addminutes":
                        return this.Value.Date.AddMinutes(getFuncNumber(0));
                    case "addseconds":
                        return this.Value.Date.AddSeconds(getFuncNumber(0));
                }
            }

            return base.Execute(lexem, environment);
        }

        static public DateTime ParseDateTime(string input, string format = null)
        {
            DateTime result;

            if (String.IsNullOrEmpty(format))
            {
                if (!DateTime.TryParseExact(input.Trim(), LexemValueDate.Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                    throw new FormatException("DateTime format is incorrect");
            }
            else
            {
                if (!DateTime.TryParseExact(input.Trim(), format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                    throw new FormatException("DateTime format is incorrect");
            }

            return result;
        }

        new public static LexemValueDate Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            if (text.Length <= 2 || text[0] != '\'')
                return null;

            var index = text.IndexOf('\'', 1);
            if (index <= 1)
                return null;

            var input = text.Substring(1, index-1).Trim();
            if (String.IsNullOrEmpty(input))
                return null;

            if (input.Equals("now", StringComparison.InvariantCultureIgnoreCase))
            {
                text = text.Crop(index + 1);
                return new LexemValueDate(DateTime.Now);
            }
            else
            {
                DateTime value;
                if (DateTime.TryParseExact(input, LexemValueDate.Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
                {
                    text = text.Crop(index + 1);
                    return new LexemValueDate(value);
                }
            }

            return null;
        }

        #region Operators
        public static implicit operator LexemValueDate(DateTime date)
        { return new LexemValueDate(date); }

        public static implicit operator DateTime(LexemValueDate argument)
        { return argument.Value; }

        public static LexemValueSpan operator -(LexemValueDate argument, LexemValueDate date)
        { return argument.Value - date.Value; }

        public static bool operator ==(LexemValueDate argument, LexemValueDate date)
        {
            if (ReferenceEquals(argument, null) || ReferenceEquals(date, null))
                return false;

            return argument.Value == date.Value;
        }

        public static bool operator !=(LexemValueDate argument, LexemValueDate date)
        { return !(argument == date); }

        public static bool operator >(LexemValueDate argument, LexemValueDate date)
        { return argument.Value > date.Value; }

        public static bool operator <(LexemValueDate argument, LexemValueDate date)
        { return date > argument; }

        public static bool operator >=(LexemValueDate argument, LexemValueDate date)
        { return argument.Value >= date.Value; }

        public static bool operator <=(LexemValueDate argument, LexemValueDate date)
        { return date >= argument; }
        #endregion
    }
}
