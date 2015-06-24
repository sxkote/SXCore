using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXLexemFunction : SXLexem
    {
        #region Variables
        protected string _name = "";
        protected List<SXExpression> _arguments;
        #endregion

        #region Properties
        public string Name
        { get { return _name; } }

        public List<SXExpression> Arguments
        { get { return _arguments; } }
        #endregion

        #region Constructors
        public SXLexemFunction(string text)
        {
            int index = text.IndexOf('(');
            if (index <= 0 || text[text.Length - 1] != ')')
                throw new FormatException("Function Lexem input is incorrect");

            _name = text.Substring(0, index);
            if (!SXLexemFunction.CheckName(this.Name))
                throw new FormatException("Function Name is incorrect");

            _arguments = new List<SXExpression>();

            var arguments_input = text.Substring(index + 1, text.Length - 1 - index - 1);

            var args = SXLexem.Split(arguments_input, new char[] { ';' }, SXLexemBracket.DefaultBrackets);
            if (args != null)
                foreach (var arg in args)
                    _arguments.Add(new SXExpression(arg));
        }
        #endregion

        #region Common
        public override string ToString()
        {
            return String.Format("{0}({1})", this.Name, String.Join(";", this.Arguments.Select(a => a.ToString()).ToList()));
        }
        #endregion

        #region Calculations
        //public bool DependsOn(string variable_name)
        //{
        //    for (int i = 0; i < this.arguments.Count; i++)
        //    {
        //        SXExpression current_argument = this.arguments[i];
        //        for (int j = 0; j < current_argument.Lexems.Count; j++)
        //        {
        //            if (current_argument.Lexems[j] is SXLexemVariable && ((SXLexemVariable)current_argument.Lexems[j]).Name == variable_name)
        //                return true;
        //            else if (current_argument.Lexems[j] is SXLexemFunction && ((SXLexemFunction)current_argument.Lexems[j]).DependsOn(variable_name))
        //                return true;
        //        }
        //    }
        //    return false;
        //}

        //public SXLexemList Diff(string variable_name)
        //{
        //    if (!this.Correct) return null;
        //    if (this.arguments.Count <= 0 || !this.Dependent(variable_name)) return new SXLexemList("0");

        //    string result = "";
        //    string argument_string = "(" + this.arguments[0].ToString() + ")";
        //    string argument_diff = "(" + this.arguments[0].Diff(variable_name).Lexems.ToString() + ")";

        //    #region Diff Power
        //    //if the name is pow:
        //    if (this.name == "pow" && this.arguments.Count == 2)
        //    {
        //        if (this.arguments[1].Lexems.Count == 1 && this.arguments[1].Lexems[0] is SXLexemNumber)
        //        {
        //            SXLexemNumber power_number = (SXLexemNumber)this.arguments[1].Lexems[0];
        //            result = "(" + "((" + power_number.ToString() + ") * pow((#X);(" + (power_number - 1).ToString() + "))) * (" + argument_diff + ")" + ")";
        //            result = result.Replace("#X", argument_string);
        //            return new SXLexemList(result);
        //        }

        //        string power_base = "(" + this.arguments[0].ToString() + ")";
        //        string power_value = "(" + this.arguments[1].ToString() + ")";
        //        SXExpression expression = new SXExpression("(exp(" + power_value + " * ln(" + power_base + ")))");
        //        return expression.Diff(variable_name).Lexems;
        //    }
        //    #endregion

        //    #region Diff Functions
        //    if (this.name == "sin")
        //        result = "cos(#X)";
        //    else if (this.name == "cos")
        //        result = "-sin(#X)";
        //    else if (this.name == "acos")
        //        result = "-1/(sqrt(1 - pow(#X;2)))";
        //    else if (this.name == "asin")
        //        result = "1/(sqrt(1 - pow(#X;2)))";
        //    else if (this.name == "tg")
        //        result = "1/(pow(cos(#X);2))";
        //    else if (this.name == "ctg")
        //        result = "-1/(pow(sin(#X);2))";
        //    else if (this.name == "atg")
        //        result = "1/(1 + pow(#X;2))";
        //    else if (this.name == "actg")
        //        result = "-1/(1 + pow(#X;2))";
        //    else if (this.name == "ch")
        //        result = "sh(#X)";
        //    else if (this.name == "sh")
        //        result = "ch(#X)";
        //    else if (this.name == "exp")
        //        result = "exp(#X)";
        //    else if (this.name == "ln")
        //        result = "1/(#X)";
        //    else if (this.name == "abs")
        //        result = "sign(#X)";
        //    else if (this.name == "sign")
        //        return new SXLexemList("0");
        //    else if (this.name == "sqrt")
        //        result = "1/(2 * sqrt(#X))";
        //    else if (this.name == "max")
        //        return new SXLexemList("0");
        //    else if (this.name == "min")
        //        return new SXLexemList("0");
        //    #endregion

        //    return new SXLexemList("((" + result.Replace("#X", argument_string) + ") * (" + argument_diff + "))");
        //}

        public SXLexemVariable Calculate(IEnvironment environment = null)
        {
            var vals = this.Arguments.Select(arg => arg.Calculate(environment).Value).ToList();

            Func<int, double> getNumber = i => ((SXLexemNumber)vals[i]).Value;

            switch (this.Name)
            {
                case "e":
                    return Math.E;
                case "pi":
                    return Math.PI;
                case "count":
                    return this.Arguments.Count;
                case "min":
                    return vals.Min(v => ((SXLexemNumber)v).Value);
                case "max":
                    return vals.Max(v => ((SXLexemNumber)v).Value);
                case "avg":
                    return vals.Average(v => ((SXLexemNumber)v).Value);
                case "sum":
                    return vals.Sum(v => ((SXLexemNumber)v).Value);
                case "sin":
                    return Math.Sin(getNumber(0));
                case "cos":
                    return Math.Cos(getNumber(0));
                case "tg":
                    return getNumber(0) == 0 ? SXLexemNumber.Infinity() : Math.Tan(getNumber(0));
                case "ctg":
                    return getNumber(0) == 0 ? SXLexemNumber.Infinity() : Math.Cos(getNumber(0)) / Math.Sin(getNumber(0));
                case "asin":
                    return Math.Asin(getNumber(0));
                case "acos":
                    return Math.Acos(getNumber(0));
                case "atg":
                    return Math.Atan(getNumber(0));
                case "actg":
                    return Math.PI / 2 - Math.Atan(getNumber(0));
                case "exp":
                    return Math.Exp(getNumber(0));
                case "ln":
                    return Math.Log(getNumber(0));
                case "abs":
                    return Math.Abs(getNumber(0));
                case "sign":
                    return Math.Sign(getNumber(0));
                case "sqrt":
                    return Math.Sqrt(getNumber(0));
                case "pow":
                    return Math.Pow(getNumber(0), getNumber(1));
                case "round":
                    return Math.Round(getNumber(0), (int)getNumber(1));
            }

            if (environment != null)
            {
                var res = environment.Calculate(this);
                if (res != null)
                    return res.Value;
            }

            throw new ArgumentException(String.Format("Function not recognized: {0}", this.Name));
        }
        #endregion

        #region Statics
        static public bool CheckName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return false;

            for (int i = 0; i < name.Length; i++)
                if (!Char.IsLetterOrDigit(name[i]) && name[i] != '_')
                    return false;

            return true;
        }

        new public static SXLexemFunction Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            string function_name = "";
            for (int i = 0; i < text.Length; i++)
                if (Char.IsLetterOrDigit(text[i]) || text[i] == '_')
                    function_name += text[i];
                else if (text[i] == '(')
                    break;
                else
                    return null;

            int index = SXLexem.Find(text, new char[] { ')' }, function_name.Length+1, SXLexemBracket.DefaultBrackets);
            if (index < 0)
                return null;

            try
            {
                var result = new SXLexemFunction(text.Substring(0, index + 1));

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
