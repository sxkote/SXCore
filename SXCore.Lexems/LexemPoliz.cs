using SXCore.Lexems.Values;
using System;
using System.Collections.Generic;

namespace SXCore.Lexems
{
    public class LexemPoliz : LexemAnalyser
    {
        protected LexemList _lexems;

        protected LexemPoliz()
        { 
            _lexems = new LexemList(); 
        }
       
        protected LexemPoliz(LexemList lexems)
        { 
            _lexems = lexems; 
        }

        public override string ToString()
        {
            return _lexems == null ? "" : _lexems.ToString();
        }

        public LexemVariable Calculate(ILexemEnvironment environment = null)
        {
            var stack = new Stack<Lexem>();

            for (int i = 0; i < _lexems.Count; i++)
            {
                if (_lexems[i] is LexemValue)
                {
                    stack.Push(_lexems[i]);
                }
                else if (_lexems[i] is LexemFunction)
                {
                    var prev = stack.Count <= 0 ? null : stack.Peek(); //i - 1 >= 0 ? _lexems[i - 1] : null;
                    var next = i + 1 < _lexems.Count ? _lexems[i + 1] : null;

                    if (prev != null && (prev is LexemValue || prev is LexemVariable) && next != null && next is LexemOperator && ((LexemOperator)next).OperatorType == LexemOperator.OperationType.Code)
                        stack.Push(_lexems[i]);
                    else
                        stack.Push((_lexems[i] as LexemFunction).Calculate(environment));
                }
                else if (_lexems[i] is LexemVariable)
                {
                    var variable = environment == null ? null : environment.Get((_lexems[i] as LexemVariable).Name);
                    stack.Push(variable == null ? _lexems[i] : variable);
                }
                else if (_lexems[i] is LexemOperator)
                {
                    var argument2 = stack.Pop();
                    var argument1 = stack.Pop();

                    stack.Push((_lexems[i] as LexemOperator).Compute(argument1, argument2, environment));
                }
                else if (_lexems[i] is LexemSwitch && _lexems[i].ToString() == "?")
                {
                    var sw = _lexems[i] as LexemSwitch;

                    var condition = LexemValue.GetValue(stack.Pop()) as LexemValueBool;
                    if (condition == null)
                        throw new InvalidOperationException("Conditional expression should return boolean value");

                    var separatorIndex = _lexems.FindPosition(sw.Simetric);
                    var endIndex = _lexems.FindPosition(sw.End);

                    if (condition.Value)
                        stack.Push(new LexemPoliz(_lexems.Range(i + 1, separatorIndex - i - 1)).Calculate(environment));
                    else
                        stack.Push(new LexemPoliz(_lexems.Range(separatorIndex + 1, endIndex - separatorIndex)).Calculate(environment));

                    i = endIndex;
                }
                else
                    stack.Push(_lexems[i]);
            }

            if (stack.Count != 1)
                throw new InvalidOperationException("Resulting lexem should be a single one");

            Lexem result = stack.Pop();
            if (result is LexemVariable)
                return (LexemVariable)result;
            else if (result is LexemValue)
                return (LexemValue)result;

            throw new InvalidOperationException("Resulting lexem should be Variable or Value type");
        }

        #region Analys
        protected override void AnalysExpression(LexemList lexems)
        {
            if (lexems != null && lexems.Count > 0 && lexems[0].ToString() == "-")
                lexems.Insert(0, 0);

            base.AnalysExpression(lexems);
        }

        protected override void OnSwitch(LexemSwitch lexem, LexemList condition, LexemList then, LexemList other)
        {
            this.AnalysLogical(condition);

            _lexems.Add(lexem);

            this.AnalysExpression(then);

            _lexems.Add(lexem.Simetric);

            this.AnalysExpression(other);

            lexem.End = _lexems[_lexems.Count - 1];
        }

        protected override void OnOperator(LexemOperator op, LexemList left, LexemList right)
        {
            switch (op.OperatorType)
            {
                case LexemOperator.OperationType.Logical:
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
                case LexemOperator.OperationType.Comparison:
                    {
                        this.AnalysArithmetic(left);
                        this.AnalysArithmetic(right);
                        _lexems.Add(op);
                        return;
                    }
                case LexemOperator.OperationType.Arithmetic:
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
                case LexemOperator.OperationType.Code:
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

        protected override void OnElement(Lexem lexem)
        {
            if (lexem is LexemValue)
                _lexems.Add(lexem);
            else if (lexem is LexemVariable)
                _lexems.Add(lexem);
            else if (lexem is LexemFunction)
                _lexems.Add(lexem);
            else
                throw new FormatException($"Expression Element not recognized: {lexem.ToString()}");
        }

        protected override void OnElement(LexemList lexems)
        {
            this.AnalysExpression(lexems.Range(1, lexems.Count - 2));
        }
        #endregion

        static public LexemPoliz Create(LexemList lexems)
        {
            var result = new LexemPoliz();
            result.Analys(lexems);
            return result;
        }
    }
}
