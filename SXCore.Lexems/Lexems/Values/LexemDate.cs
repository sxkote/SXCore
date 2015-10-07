using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXLexemDate : SXLexemValue
    {
        static public string[] Formats = { "dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy HH:mm", "dd.MM.yyyy", "dd.MM.yy HH:mm:ss", "dd.MM.yy HH:mm", "dd.MM.yy", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm" };

        public enum TenseType { Past, Present, Future };

        #region variables
        protected TenseType _tense = TenseType.Present;
        protected DateTime _value = DateTime.Now;
        #endregion

        #region Properties
        public override ValueType Type
        { get { return ValueType.Date; } }

        public TenseType Tense
        { get { return _tense; } }

        public DateTime Value
        { get { return _value; } }
        #endregion

        #region Constructors
        public SXLexemDate(string text)
        {
            var input = String.IsNullOrEmpty(text) ? null : text.Trim().ToLower();

            if (String.IsNullOrEmpty(input) || input.Length < 2 || input[0] != '\'' || input[input.Length - 1] != '\'')
                throw new FormatException("Date Value input can't be empty");

            input = input.Substring(1, input.Length - 2).Trim().ToLower();
            if (String.IsNullOrEmpty(input))
                throw new FormatException("Date Value input can't be empty");

            if (input == "past")
            {
                _tense = TenseType.Past;
                _value = DateTime.MinValue;
            }
            else if (input == "future")
            {
                _tense = TenseType.Future;
                _value = DateTime.MaxValue;
            }
            else if (input == "now")
            {
                _tense = TenseType.Present;
                _value = DateTime.Now;
            }
            else
            {
                _tense = TenseType.Present;
                if (!DateTime.TryParseExact(input, SXLexemDate.Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out _value))
                    throw new FormatException("Date Value is in wrong format");
            }
        }

        public SXLexemDate(DateTime date)
        {
            _tense = TenseType.Present;
            _value = date;
        }
        #endregion

        #region Common
        public override string ToString()
        {
            switch (this.Tense)
            {
                case TenseType.Past:
                    return "'past'";
                case TenseType.Future:
                    return "'future'";
                default:
                    return String.Format("'{0}'", this.Value.ToString(Formats.First()));
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is SXLexemDate)
            {
                var date = obj as SXLexemDate;
                if (date.Tense != this.Tense)
                    return false;

                if (date.Tense == TenseType.Present)
                    return date.Value == this.Value;

                return true;
            }

            if (obj is DateTime) 
                return (this.Tense == TenseType.Present && this.Value == (DateTime)obj);

            return false;
        }

        public override int GetHashCode()
        { return this.Value.GetHashCode(); }
        #endregion

        #region Operators
        public static implicit operator SXLexemDate(DateTime date)
        { return new SXLexemDate(date); }

        public static implicit operator DateTime(SXLexemDate argument)
        {
            if (argument.Tense == TenseType.Past)
                return DateTime.MinValue;
            if (argument.Tense == TenseType.Future)
                return DateTime.MaxValue;

            return argument.Value;
        }

        public static SXLexemSpan operator -(SXLexemDate argument, SXLexemDate date)
        {
            if (argument.Tense == TenseType.Future)
            {
                if (date.Tense == TenseType.Future)
                    return new TimeSpan(0, 0, 0, 0);

                return new TimeSpan(Int32.MaxValue, 0, 0, 0);
            }
            else if (argument.Tense == TenseType.Past)
            {
                if (date.Tense == TenseType.Past)
                    return new TimeSpan(0, 0, 0, 0);

                return new TimeSpan(Int32.MinValue, 0, 0, 0);
            }
            else
            {
                if (date.Tense == TenseType.Past)
                    return new TimeSpan(Int32.MaxValue, 0, 0, 0);
                else if (date.Tense == TenseType.Future)
                    return new TimeSpan(Int32.MinValue, 0, 0, 0);

                return argument.Value - date.Value;
            }
        }

        public static bool operator ==(SXLexemDate argument, SXLexemDate date)
        {
            if (argument.Tense != date.Tense) 
                return false;

            if (argument.Tense != TenseType.Present)
                return true;

            return argument.Value == date.Value;
        }

        public static bool operator !=(SXLexemDate argument, SXLexemDate date)
        {
            if (argument.Tense != date.Tense)
                return true;

            if (argument.Tense != TenseType.Present)
                return false;

            return argument.Value != date.Value;
        }

        public static bool operator >(SXLexemDate argument, SXLexemDate date)
        {
            if (argument.Tense == TenseType.Future)
                return (date.Tense != TenseType.Future);

            if (argument.Tense == TenseType.Past)
                return false;

            if (date.Tense == TenseType.Present)
                return (argument.Value > date.Value);

            return (date.Tense == TenseType.Past);
        }

        public static bool operator <(SXLexemDate argument, SXLexemDate date)
        { return date > argument; }

        public static bool operator >=(SXLexemDate argument, SXLexemDate date)
        {
            if (argument.Tense == TenseType.Future)
                return true;

            if (argument.Tense == TenseType.Past)
                return (date.Tense == TenseType.Past);

            if (date.Tense == TenseType.Present)
                return (argument.Value >= date.Value);

            return (date.Tense == TenseType.Past);
        }

        public static bool operator <=(SXLexemDate argument, SXLexemDate date)
        { return date >= argument; }
        #endregion

        #region Calculations
        public override SXLexemVariable Execute(SXLexem lexem, IEnvironment environment = null)
        {
            if (lexem == null) return null;

            if (lexem is SXLexemVariable)
            {
                switch (((SXLexemVariable)lexem).Name.ToLower())
                {
                    case "date": 
                        return  this.Value.Date;
                    case "year":
                        return  this.Value.Year;
                    case "month":
                        return  this.Value.Month;
                    case "day":
                        return  this.Value.Day;
                    case "hour":
                        return  this.Value.Hour;
                    case "minute":
                        return  this.Value.Minute;
                    case "second":
                        return  this.Value.Second;
                    case "dayofweek":
                        return  this.Value.DayOfWeek.ToString();
                }
            }
            else if (lexem is SXLexemFunction)
            {
                var func = lexem as SXLexemFunction;

                Func<int, double> getFuncNumber = i=>(func.Arguments[i].Calculate(environment).Value as SXLexemNumber).Value;
                
                switch (((SXLexemFunction)lexem).Name.ToLower())
                {
                    case "tostring":
                        return this.Value.ToString(Formats.First());
                    case "addyears":
                        return  this.Value.Date.AddYears((int)getFuncNumber(0));
                    case "addmonths":
                        return  this.Value.Date.AddMonths((int)getFuncNumber(0));
                    case "adddays":
                        return  this.Value.Date.AddDays(getFuncNumber(0));
                    case "addhours":
                        return  this.Value.Date.AddHours(getFuncNumber(0));
                    case "addminutes":
                        return  this.Value.Date.AddMinutes(getFuncNumber(0));
                    case "addseconds":
                        return  this.Value.Date.AddSeconds(getFuncNumber(0));
                }
            }

            return base.Execute(lexem, environment);
        }
        #endregion

        #region Statics
        static public DateTime ParseDatetime(string input, string format = null)
        {
            DateTime result;

            if (String.IsNullOrEmpty(format))
            {
                if (!DateTime.TryParseExact(input.Trim(), SXLexemDate.Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                    throw new FormatException("DateTime is in wrong format");
            }
            else
            {
                if (!DateTime.TryParseExact(input.Trim(), format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                    throw new FormatException("DateTime is in wrong format");
            }

            return result;
        }

        new public static SXLexemDate Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            if (text.Length <= 2 || text[0] != '\'')
                return null;

            var index = text.IndexOf('\'', 1);
            if (index <= 0)
                return null;

            var input = text.Substring(0, index + 1);

            try
            {
                var result = new SXLexemDate(input);

                text = text.Crop(index + 1);

                return result;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
