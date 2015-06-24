using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXLexemOperator : SXLexemSymbol
    {
        static public string[] Operators = { "+", "-", "*", "/", "<=", ">=", "<", ">", "==", "!=", "||", "&&", "=", "->", "." };

        public enum OperationType { Arithmetic, Comparison, Logical, Code };

        #region Properties
        public override SymbolType Type
        { get { return SymbolType.Operator; } }

        public OperationType OperatorType
        {
            get
            {
                switch (this.Text)
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
                }

                return OperationType.Code;
            }
        }
        #endregion

        #region Constructor
        public SXLexemOperator(string text)
        {
            _text = text.Trim();

            if (!Operators.Contains(this.Text))
                throw new FormatException("Unknown Operator input");
        }
        #endregion

        #region Calculations
        public SXLexemVariable Compute(SXLexem argument1, SXLexem argument2, IEnvironment environment = null)
        {
            SXLexemValue result = null;

            if (this.OperatorType == OperationType.Arithmetic)
            {
                #region Arithmetic Operations
                SXLexemValue arg1 = SXLexemValue.GetValue(argument1);
                SXLexemValue arg2 = SXLexemValue.GetValue(argument2);
                if (arg1 == null || arg2 == null) return null;

                if (arg1 is SXLexemNumber && arg2 is SXLexemNumber)
                {
                    #region number
                    SXLexemNumber val1 = (SXLexemNumber)arg1;
                    SXLexemNumber va12 = (SXLexemNumber)arg2;

                    if (this.Text == "+") result = val1 + va12;
                    else if (this.Text == "-") result = val1 - va12;
                    else if (this.Text == "*") result = val1 * va12;
                    else if (this.Text == "/") result = val1 / va12;
                    #endregion
                }
                else if (arg1 is SXLexemComplex && arg2 is SXLexemComplex)
                {
                    #region complex
                    SXLexemComplex val1 = (SXLexemComplex)arg1;
                    SXLexemComplex val2 = (SXLexemComplex)arg2;

                    if (this.Text == "+") result = val1 + val2;
                    else if (this.Text == "-") result = val1 - val2;
                    else if (this.Text == "*") result = val1 * val2;
                    else if (this.Text == "/") result = val1 / val2;

                    if (result != null && result is SXLexemComplex && ((SXLexemComplex)result).Im == 0)
                        result = ((SXLexemComplex)result).Re;
                    #endregion
                }
                else if (arg1 is SXLexemDate && arg2 is SXLexemSpan)
                {
                    #region date and span
                    SXLexemDate date = (SXLexemDate)arg1;
                    SXLexemSpan span = (SXLexemSpan)arg2;

                    if (date.Tense != SXLexemDate.TenseType.Present) result = date;
                    else if (this.Text == "+") result = date.Value + span.Value;
                    else if (this.Text == "-") result = date.Value - span.Value;
                    #endregion
                }
                else if (arg1 is SXLexemSpan && arg2 is SXLexemDate)
                {
                    #region span and date
                    SXLexemDate date = (SXLexemDate)arg2;
                    SXLexemSpan span = (SXLexemSpan)arg1;

                    if (date.Tense != SXLexemDate.TenseType.Present) result = date;
                    else if (this.Text == "+") result = date.Value + span.Value;
                    #endregion
                }
                else if (arg1 is SXLexemDate && arg2 is SXLexemDate)
                {
                    if (this.Text == "-")
                        result = ((SXLexemDate)arg1) - ((SXLexemDate)arg2);
                }
                else if (arg1 is SXLexemText && arg2 is SXLexemText)
                {
                    if (this.Text == "+")
                        result = ((SXLexemText)arg1) + ((SXLexemText)arg2);
                }
                #endregion
            }
            else if (this.OperatorType == OperationType.Logical)
            {
                #region Logical Operations
                SXLexemValue arg1 = SXLexemValue.GetValue(argument1);
                SXLexemValue arg2 = SXLexemValue.GetValue(argument2);
                if (arg1 == null || arg2 == null) return null;

                if (arg1 is SXLexemBool && arg2 is SXLexemBool)
                {
                    if (this.Text == "||")
                        result = ((SXLexemBool)arg1) || ((SXLexemBool)arg2);
                    else if (this.Text == "&&")
                        result = ((SXLexemBool)arg1) && ((SXLexemBool)arg2);
                    else
                        result = new SXLexemBool(SXLexemBool.BoolType.Unknown);
                }
                #endregion
            }
            else if (this.OperatorType == OperationType.Comparison)
            {
                #region Comparison Operations
                SXLexemValue arg1 = SXLexemValue.GetValue(argument1);
                SXLexemValue arg2 = SXLexemValue.GetValue(argument2);
                if (arg1 == null || arg2 == null) return null;

                if (arg1 is SXLexemNumber && arg2 is SXLexemNumber)
                {
                    #region number
                    SXLexemNumber number1 = (SXLexemNumber)arg1;
                    SXLexemNumber number2 = (SXLexemNumber)arg2;

                    if (this.Text == ">") result = number1 > number2;
                    else if (this.Text == ">=") result = number1 >= number2;
                    else if (this.Text == "<") result = number1 < number2;
                    else if (this.Text == "<=") result = number1 <= number2;
                    else if (this.Text == "==") result = number1 == number2;
                    else if (this.Text == "!=") result = number1 != number2;
                    #endregion
                }
                else if (arg1 is SXLexemComplex && arg2 is SXLexemComplex)
                {
                    #region complex
                    SXLexemComplex complex1 = (SXLexemComplex)arg1;
                    SXLexemComplex complex2 = (SXLexemComplex)arg2;

                    if (this.Text == ">") result = complex1 > complex2;
                    else if (this.Text == ">=") result = complex1 >= complex2;
                    else if (this.Text == "<") result = complex1 < complex2;
                    else if (this.Text == "<=") result = complex1 <= complex2;
                    else if (this.Text == "==") result = complex1 == complex2;
                    else if (this.Text == "!=") result = complex1 != complex2;
                    #endregion
                }
                else if (arg1 is SXLexemText && arg2 is SXLexemText)
                {
                    #region text
                    SXLexemText text_1 = (SXLexemText)arg1;
                    SXLexemText text_2 = (SXLexemText)arg2;

                    if (this.Text == ">") result = text_1 > text_2;
                    else if (this.Text == ">=") result = text_1 >= text_2;
                    else if (this.Text == "<") result = text_1 < text_2;
                    else if (this.Text == "<=") result = text_1 <= text_2;
                    else if (this.Text == "==") result = text_1 == text_2;
                    else if (this.Text == "!=") result = text_1 != text_2;
                    #endregion
                }
                else if (arg1 is SXLexemDate && arg2 is SXLexemDate)
                {
                    #region date
                    SXLexemDate date1 = (SXLexemDate)arg1;
                    SXLexemDate date2 = (SXLexemDate)arg2;

                    if (this.Text == ">") result = date1 > date2;
                    else if (this.Text == ">=") result = date1 >= date2;
                    else if (this.Text == "<") result = date1 < date2;
                    else if (this.Text == "<=") result = date1 <= date2;
                    else if (this.Text == "==") result = date1 == date2;
                    else if (this.Text == "!=") result = date1 != date2;
                    #endregion
                }
                else if (arg1 is SXLexemSpan && arg2 is SXLexemSpan)
                {
                    #region span
                    SXLexemSpan span1 = (SXLexemSpan)arg1;
                    SXLexemSpan span2 = (SXLexemSpan)arg2;

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
                if (argument1 is SXLexemVariable)
                    return ((SXLexemVariable)argument1).Execute(argument2, environment);
                else if (argument1 is SXLexemValue)
                    return ((SXLexemValue)argument1).Execute(argument2, environment);
                else
                    return null;
                #endregion
            }

            return result;
        }
        #endregion

        #region Statics
        new static public SXLexemOperator Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            var input = text;
            var item = Operators.OrderByDescending(op => op.Length).FirstOrDefault(o => input.StartsWith(o));
            if (String.IsNullOrEmpty(item))
                return null;

            text = text.Crop(item.Length);

            return new SXLexemOperator(item);
        }
        #endregion
    }
}
