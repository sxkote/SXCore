using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXPoliz : SXAnalyser
    {
        #region variables
        protected SXLexemList _lexems;
        #endregion

        #region Constructors
        protected SXPoliz()
        { 
            _lexems = new SXLexemList(); 
        }
       
        protected SXPoliz(SXLexemList lexems)
        { 
            _lexems = lexems; 
        }
        #endregion

        #region Common
        public override string ToString()
        {
            return _lexems == null ? "" : _lexems.ToString();
        }
        #endregion

        #region Analys
        protected override void AnalysExpression(SXLexemList lexems)
        {
            if (lexems != null && lexems.Count > 0 && lexems[0].ToString() == "-")
                lexems.Insert(0, 0);

            base.AnalysExpression(lexems);
        }

        protected override void OnSwitch(SXLexemSwitch lexem, SXLexemList condition, SXLexemList then, SXLexemList other)
        {
            this.AnalysLogical(condition);

            _lexems.Add(lexem);

            this.AnalysExpression(then);

            _lexems.Add(lexem.Simetric);

            this.AnalysExpression(other);

            lexem.End = _lexems[_lexems.Count - 1];
        }

        protected override void OnOperator(SXLexemOperator op, SXLexemList left, SXLexemList right)
        {
            switch (op.OperatorType)
            {
                case SXLexemOperator.OperationType.Logical:
                    {
                        if (op.Text == "||")
                        {
                            this.AnalysLogicalOr(left);
                            this.AnalysLogicalAnd(right);
                            _lexems.Add(op);
                        }
                        else if (op.Text == "&&")
                        {
                            this.AnalysLogicalAnd(left);
                            this.AnalysComparison(right);
                            _lexems.Add(op);
                        }
                        return;
                    }
                case SXLexemOperator.OperationType.Comparison:
                    {
                        this.AnalysArithmetic(left);
                        this.AnalysArithmetic(right);
                        _lexems.Add(op);
                        return;
                    }
                case SXLexemOperator.OperationType.Arithmetic:
                    {
                        if (op.Text == "+" || op.Text == "-")
                        {
                            this.AnalysArithmeticAddition(left);
                            this.AnalysArithmeticMultiplication(right);
                            _lexems.Add(op);
                        }
                        else if (op.Text == "*" || op.Text == "/")
                        {
                            this.AnalysArithmeticMultiplication(left);
                            this.AnalysElement(right);
                            _lexems.Add(op);
                        }
                        return;
                    }
                case SXLexemOperator.OperationType.Code:
                    {
                        if (op.Text == "->" || op.Text == ".")
                        {
                            this.AnalysElement(left);
                            this.AnalysElement(right);
                            _lexems.Add(op);
                        }
                        return;
                    }
            }
        }

        protected override void OnElement(SXLexem lexem)
        {
            if (lexem is SXLexemValue)
                _lexems.Add(lexem);
            else if (lexem is SXLexemVariable)
                _lexems.Add(lexem);
            else if (lexem is SXLexemFunction)
                _lexems.Add(lexem);
            else
                throw new FormatException(String.Format("Expression Element not recognized: {0}", lexem.ToString()));
        }

        protected override void OnElement(SXLexemList lexems)
        {
            this.AnalysExpression(lexems.Range(1, lexems.Count - 2));
        }
        #endregion

        #region Calculations
        public SXLexemVariable Calculate(IEnvironment environment = null)
        {
            var stack = new Stack<SXLexem>();
            for (int i = 0; i < _lexems.Count; i++)
            {
                if (_lexems[i] is SXLexemValue)
                {
                    stack.Push(_lexems[i]);
                }
                else if (_lexems[i] is SXLexemFunction)
                {
                    var prev = i - 1 >= 0 ? _lexems[i - 1] : null;
                    var next = i + 1 < _lexems.Count ? _lexems[i + 1] : null;

                    if (prev != null && (prev is SXLexemValue || prev is SXLexemVariable) && next != null && next is SXLexemOperator && ((SXLexemOperator)next).OperatorType == SXLexemOperator.OperationType.Code)
                        stack.Push(_lexems[i]);
                    else
                        stack.Push((_lexems[i] as SXLexemFunction).Calculate(environment));
                }
                else if (_lexems[i] is SXLexemVariable)
                {
                    var variable = environment == null ? null : environment.Get((_lexems[i] as SXLexemVariable).Name);
                    stack.Push(variable == null ? _lexems[i] : variable);
                }
                else if (_lexems[i] is SXLexemOperator)
                {
                    var argument2 = stack.Pop();
                    var argument1 = stack.Pop();

                    stack.Push((_lexems[i] as SXLexemOperator).Compute(argument1, argument2, environment));
                }
                else if (_lexems[i] is SXLexemSwitch && _lexems[i].ToString() == "?")
                {
                    var sw = _lexems[i] as SXLexemSwitch;

                    var condition = SXLexemValue.GetValue(stack.Pop()) as SXLexemBool;
                    if (condition == null) return null;

                    var separatorIndex = _lexems.FindPosition(sw.Simetric);
                    var endIndex = _lexems.FindPosition(sw.End);

                    if (condition.Value == SXLexemBool.BoolType.True)
                        stack.Push(new SXPoliz(_lexems.Range(i + 1, separatorIndex - i - 1)).Calculate(environment));
                    else
                        stack.Push(new SXPoliz(_lexems.Range(separatorIndex + 1, endIndex - separatorIndex)).Calculate(environment));

                    i = endIndex;
                }
                else
                    stack.Push(_lexems[i]);
            }

            if (stack.Count != 1) return null;

            SXLexem result = stack.Pop();
            if (result is SXLexemVariable)
                return (SXLexemVariable)result;
            else if (result is SXLexemValue)
                return (SXLexemValue)result;

            return null;
        }
        #endregion

        #region Statics
        static public SXPoliz Create(SXLexemList lexems)
        {
            var result = new SXPoliz();
            result.Analys(lexems);
            return result;
        }
        #endregion
    }
}
