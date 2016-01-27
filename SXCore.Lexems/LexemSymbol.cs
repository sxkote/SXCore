using SXCore.Lexems.Values;
using System;
using System.Linq;

namespace SXCore.Lexems
{
    public enum LexemSymbolType { Comma, Bracket, Operator, Switch };

    public abstract class LexemSymbol : Lexem
    {
        public string Text { get; protected set; }
        public abstract LexemSymbolType Type { get; }

        public LexemSymbol(string text)
        {
            if (String.IsNullOrEmpty(text))
                throw new ArgumentException("Symbol can't be null or empty");

            this.Text = text.Trim();
        }

        public override string ToString()
        { return this.Text ?? ""; }

        new static public LexemSymbol Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            char ch = text[0];

            switch (ch)
            {
                // Switch
                case '?':
                case ':':
                    {
                        text = text.Crop(1);
                        return new LexemSwitch(ch.ToString());
                    }

                // Brackets
                case '(':
                case ')':
                case '{':
                case '}':
                case '[':
                case ']':
                    {
                        text = text.Crop(1);
                        return new LexemBracket(ch.ToString());
                    }

                // Commas
                case ',':
                case ';':
                    {
                        text = text.Crop(1);
                        return new LexemComma(ch.ToString());
                    }

                // Operators
                case '+':
                case '-':
                case '*':
                case '/':
                case '<':
                case '>':
                case '=':
                case '!':
                case '&':
                case '|':
                case '.':
                    {
                        return LexemOperator.Parse(ref text);
                    }

                // Invalid Symbol
                default: return null;
            }
        }

        // Parse (old version) - performance is not so fast in compare with switch { case : }...
        //new static public LexemSymbol Parse(ref string text)
        //{
        //    if (String.IsNullOrEmpty(text))
        //        return null;

        //    text = text.Trim();

        //    LexemSymbol result = null;

        //    if ((result = LexemOperator.Parse(ref text)) != null)
        //        return result;

        //    if ((result = LexemBracket.Parse(ref text)) != null)
        //        return result;

        //    if ((result = LexemComma.Parse(ref text)) != null)
        //        return result;

        //    if ((result = LexemSwitch.Parse(ref text)) != null)
        //        return result;

        //    return null;
        //}
    }

    public class LexemBracket : LexemSymbol
    {
        static public SymbolPair[] Brackets = { new SymbolPair('(', ')'), new SymbolPair('{', '}'), new SymbolPair('[', ']') };

        public override LexemSymbolType Type
        { get { return LexemSymbolType.Bracket; } }

        private LexemBracket _simetric = null;
        public LexemBracket Simetric
        {
            get { return _simetric; }
            set
            {
                _simetric = value;
                if (_simetric != null && _simetric.Simetric != this)
                    _simetric.Simetric = this;
            }
        }

        public LexemBracket(string text)
            : base(text)
        {
            if (!Brackets.Any(br => br.Contains(this.Text)))
                throw new FormatException("Unknown Bracket input");
        }

        new static public LexemBracket Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            var input = text;
            var item = Brackets.SelectMany(p => new string[] { p.Open, p.Close }).FirstOrDefault(symb => input.StartsWith(symb));
            if (String.IsNullOrEmpty(item))
                return null;

            text = text.Crop(item.Length);

            return new LexemBracket(item);
        }
    }

    public class LexemComma : LexemSymbol
    {
        static public string[] Commas = { ",", ";" };

        public override LexemSymbolType Type
        { get { return LexemSymbolType.Comma; } }

        public LexemComma(string text)
            : base(text)
        {
            if (!Commas.Contains(this.Text))
                throw new FormatException("Unknown Comma input");
        }

        new static public LexemComma Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            var input = text;
            var item = Commas.FirstOrDefault(o => input.StartsWith(o));
            if (String.IsNullOrEmpty(item))
                return null;

            text = text.Crop(item.Length);

            return new LexemComma(item);
        }
    }

    public class LexemOperator : LexemSymbol
    {
        public enum OperationType { Arithmetic, Comparison, Logical, Code };

        static public string[] Operators = { "+", "-", "*", "/", "<=", ">=", "<", ">", "==", "!=", "||", "&&", "=", "->", "." };

        public override LexemSymbolType Type
        { get { return LexemSymbolType.Operator; } }
        public OperationType OperatorType { get; private set; }

        public LexemOperator(string text)
            : base(text)
        {
            if (!Operators.Contains(this.Text))
                throw new FormatException("Unknown Operator input");

            this.OperatorType = DefineOperationType(this.Text);
        }

        public LexemVariable Compute(Lexem argument1, Lexem argument2, ILexemEnvironment environment = null)
        {
            LexemValue result = null;

            if (this.OperatorType == OperationType.Arithmetic)
            {
                #region Arithmetic Operations
                LexemValue arg1 = LexemValue.GetValue(argument1);
                LexemValue arg2 = LexemValue.GetValue(argument2);
                if (arg1 == null || arg2 == null) return null;

                if (arg1 is LexemValueNumber && arg2 is LexemValueNumber)
                {
                    #region number
                    LexemValueNumber val1 = (LexemValueNumber)arg1;
                    LexemValueNumber va12 = (LexemValueNumber)arg2;

                    if (this.Text == "+") result = val1 + va12;
                    else if (this.Text == "-") result = val1 - va12;
                    else if (this.Text == "*") result = val1 * va12;
                    else if (this.Text == "/") result = val1 / va12;
                    #endregion
                }
                else if (arg1 is LexemValueComplex && arg2 is LexemValueComplex)
                {
                    #region complex
                    LexemValueComplex val1 = (LexemValueComplex)arg1;
                    LexemValueComplex val2 = (LexemValueComplex)arg2;

                    if (this.Text == "+") result = val1 + val2;
                    else if (this.Text == "-") result = val1 - val2;
                    else if (this.Text == "*") result = val1 * val2;
                    else if (this.Text == "/") result = val1 / val2;

                    if (result != null && result is LexemValueComplex && ((LexemValueComplex)result).Im == 0)
                        result = ((LexemValueComplex)result).Re;
                    #endregion
                }
                else if (arg1 is LexemValueDate && arg2 is LexemValueSpan)
                {
                    #region date and span
                    LexemValueDate date = (LexemValueDate)arg1;
                    LexemValueSpan span = (LexemValueSpan)arg2;

                    if (this.Text == "+")
                        result = date.Value + span.Value;
                    else if (this.Text == "-")
                        result = date.Value - span.Value;
                    #endregion
                }
                else if (arg1 is LexemValueSpan && arg2 is LexemValueDate)
                {
                    #region span and date
                    LexemValueDate date = (LexemValueDate)arg2;
                    LexemValueSpan span = (LexemValueSpan)arg1;

                    if (this.Text == "+")
                        result = date.Value + span.Value;
                    #endregion
                }
                else if (arg1 is LexemValueDate && arg2 is LexemValueDate)
                {
                    if (this.Text == "-")
                        result = ((LexemValueDate)arg1) - ((LexemValueDate)arg2);
                }
                else if (arg1 is LexemValueText && arg2 is LexemValueText)
                {
                    if (this.Text == "+")
                        result = ((LexemValueText)arg1) + ((LexemValueText)arg2);
                }
                #endregion
            }
            else if (this.OperatorType == OperationType.Logical)
            {
                #region Logical Operations
                LexemValue arg1 = LexemValue.GetValue(argument1);
                LexemValue arg2 = LexemValue.GetValue(argument2);
                if (arg1 == null || arg2 == null) return null;

                if (arg1 is LexemValueBool && arg2 is LexemValueBool)
                {
                    if (this.Text == "||")
                        result = ((LexemValueBool)arg1) || ((LexemValueBool)arg2);
                    else if (this.Text == "&&")
                        result = ((LexemValueBool)arg1) && ((LexemValueBool)arg2);
                    else
                        throw new InvalidOperationException($"Invalid operation {this.Text} between booleans");
                }
                #endregion
            }
            else if (this.OperatorType == OperationType.Comparison)
            {
                #region Comparison Operations
                LexemValue arg1 = LexemValue.GetValue(argument1);
                LexemValue arg2 = LexemValue.GetValue(argument2);
                if (arg1 == null || arg2 == null) return null;

                if (arg1 is LexemValueNumber && arg2 is LexemValueNumber)
                {
                    #region number
                    LexemValueNumber number1 = (LexemValueNumber)arg1;
                    LexemValueNumber number2 = (LexemValueNumber)arg2;

                    if (this.Text == ">") result = number1 > number2;
                    else if (this.Text == ">=") result = number1 >= number2;
                    else if (this.Text == "<") result = number1 < number2;
                    else if (this.Text == "<=") result = number1 <= number2;
                    else if (this.Text == "==") result = number1 == number2;
                    else if (this.Text == "!=") result = number1 != number2;
                    #endregion
                }
                else if (arg1 is LexemValueComplex && arg2 is LexemValueComplex)
                {
                    #region complex
                    LexemValueComplex complex1 = (LexemValueComplex)arg1;
                    LexemValueComplex complex2 = (LexemValueComplex)arg2;

                    if (this.Text == ">") result = complex1 > complex2;
                    else if (this.Text == ">=") result = complex1 >= complex2;
                    else if (this.Text == "<") result = complex1 < complex2;
                    else if (this.Text == "<=") result = complex1 <= complex2;
                    else if (this.Text == "==") result = complex1 == complex2;
                    else if (this.Text == "!=") result = complex1 != complex2;
                    #endregion
                }
                else if (arg1 is LexemValueText && arg2 is LexemValueText)
                {
                    #region text
                    LexemValueText text_1 = (LexemValueText)arg1;
                    LexemValueText text_2 = (LexemValueText)arg2;

                    if (this.Text == ">") result = text_1 > text_2;
                    else if (this.Text == ">=") result = text_1 >= text_2;
                    else if (this.Text == "<") result = text_1 < text_2;
                    else if (this.Text == "<=") result = text_1 <= text_2;
                    else if (this.Text == "==") result = text_1 == text_2;
                    else if (this.Text == "!=") result = text_1 != text_2;
                    #endregion
                }
                else if (arg1 is LexemValueDate && arg2 is LexemValueDate)
                {
                    #region date
                    LexemValueDate date1 = (LexemValueDate)arg1;
                    LexemValueDate date2 = (LexemValueDate)arg2;

                    if (this.Text == ">") result = date1 > date2;
                    else if (this.Text == ">=") result = date1 >= date2;
                    else if (this.Text == "<") result = date1 < date2;
                    else if (this.Text == "<=") result = date1 <= date2;
                    else if (this.Text == "==") result = date1 == date2;
                    else if (this.Text == "!=") result = date1 != date2;
                    #endregion
                }
                else if (arg1 is LexemValueSpan && arg2 is LexemValueSpan)
                {
                    #region span
                    LexemValueSpan span1 = (LexemValueSpan)arg1;
                    LexemValueSpan span2 = (LexemValueSpan)arg2;

                    if (this.Text == ">") result = span1 > span2;
                    else if (this.Text == ">=") result = span1 >= span2;
                    else if (this.Text == "<") result = span1 < span2;
                    else if (this.Text == "<=") result = span1 <= span2;
                    else if (this.Text == "==") result = span1 == span2;
                    else if (this.Text == "!=") result = span1 != span2;
                    #endregion
                }
                #endregion
            }
            else if (this.OperatorType == OperationType.Code)
            {
                #region Code Operations
                if (this.Text != "->" && this.Text != ".") return null;
                if (argument1 is LexemVariable)
                    return ((LexemVariable)argument1).Execute(argument2, environment);
                else if (argument1 is LexemValue)
                    return ((LexemValue)argument1).Execute(argument2, environment);
                else
                    return null;
                #endregion
            }

            return result;
        }

        new static public LexemOperator Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            var input = text;
            var item = Operators.OrderByDescending(op => op.Length).FirstOrDefault(o => input.StartsWith(o));
            if (String.IsNullOrEmpty(item))
                return null;

            text = text.Crop(item.Length);

            return new LexemOperator(item);
        }

        static public OperationType DefineOperationType(string text)
        {
            if (String.IsNullOrEmpty(text))
                return OperationType.Code;

            switch (text)
            {
                case "+":
                case "-":
                case "*":
                case "/":
                    return OperationType.Arithmetic;

                case "<=":
                case ">=":
                case "<":
                case ">":
                case "==":
                case "!=":
                    return OperationType.Comparison;

                case "||":
                case "&&":
                    return OperationType.Logical;

                case "=":
                case "->":
                case ".":
                    return OperationType.Code;

                default:
                    return OperationType.Code;
            }
        }
    }

    public class LexemSwitch : LexemSymbol
    {
        public override LexemSymbolType Type
        { get { return LexemSymbolType.Switch; } }

        private LexemSwitch _simetric = null;
        public LexemSwitch Simetric
        {
            get { return _simetric; }
            set
            {
                _simetric = value;
                if (_simetric != null && _simetric.Simetric != this)
                    _simetric.Simetric = this;
            }
        }

        private Lexem _end = null;
        public Lexem End
        {
            get { return _end; }
            set
            {
                _end = value;
                if (this.Simetric != null && this.Simetric.End != value)
                    this.Simetric.End = value;
            }
        }

        public LexemSwitch(string text)
                : base(text)
        {
            if (this.Text != "?" && this.Text != ":")
                throw new FormatException("Unknown Switch input");
        }

        new static public LexemSwitch Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();
            if (!text.StartsWith("?") && !text.StartsWith(":"))
                return null;

            var result = new LexemSwitch(text[0].ToString());

            text = text.Crop(1);

            return result;
        }
    }
}
