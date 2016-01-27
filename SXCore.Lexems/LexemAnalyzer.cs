using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public abstract class LexemAnalyser
    {
        #region Analys
        protected virtual void Analys(LexemList lexems)
        { this.AnalysExpression(lexems); }

        protected virtual void AnalysExpression(LexemList lexems)
        {
            for (int i = 0; i < lexems.Count; i++)
            {
                var lexem = lexems[i];

                if (lexem is LexemBracket)
                {
                    i = this.SkipBrackets(lexems, i);
                }
                else if (lexem is LexemSwitch && lexem.ToString() == "?")
                {
                    var sw = lexem as LexemSwitch;

                    var simetric = sw.Simetric;
                    if (simetric == null)
                        throw new FormatException("Switch syntax should have simetric Else path");

                    int index = lexems.FindPosition(simetric);
                    if (index <= i)
                        throw new FormatException("Switch syntax balance is incorrect");

                    var condition = lexems.Range(0, i);
                    var then = lexems.Range(i + 1, index - i - 1);
                    var other = lexems.Range(index + 1, lexems.Count - index - 1);

                    this.OnSwitch(sw, condition, then, other);

                    return;
                }
            }

            this.AnalysLogical(lexems);
        }

        protected void AnalysLogical(LexemList lexems)
        { this.AnalysLogicalOr(lexems); }

        protected virtual void AnalysLogicalOr(LexemList lexems)
        {
            this.Scan(lexems, LexemOperator.OperationType.Logical, new string[] { "||" }, this.AnalysLogicalAnd);
        }

        protected virtual void AnalysLogicalAnd(LexemList lexems)
        {
            this.Scan(lexems, LexemOperator.OperationType.Logical, new string[] { "&&" }, this.AnalysComparison);
        }

        protected virtual void AnalysComparison(LexemList lexems)
        {
            this.Scan(lexems, LexemOperator.OperationType.Comparison, null, this.AnalysArithmetic);
        }

        protected void AnalysArithmetic(LexemList lexems)
        { this.AnalysArithmeticAddition(lexems); }

        protected virtual void AnalysArithmeticAddition(LexemList lexems)
        {
            this.Scan(lexems, LexemOperator.OperationType.Arithmetic, new string[] { "+", "-" }, this.AnalysArithmeticMultiplication);
        }

        protected virtual void AnalysArithmeticMultiplication(LexemList lexems)
        {
            this.Scan(lexems, LexemOperator.OperationType.Arithmetic, new string[] { "*", "/" }, this.AnalysElement);
        }

        protected virtual void AnalysElement(LexemList lexems)
        {
            if (lexems == null || lexems.Count <= 0)
                throw new FormatException("Analys Element is empty");

            if (lexems.Count == 1)
            {
                this.OnElement(lexems[0]);
                return;
            }

            if (lexems[0] is LexemBracket && lexems.FindPosition((lexems[0] as LexemBracket).Simetric) == lexems.Count - 1)
            {
                this.OnElement(lexems);
                return;
            }

            this.Scan(lexems, LexemOperator.OperationType.Code, new string[] { "->", "." }, this.Error);
        }
        #endregion

        #region Functions
        protected void Scan(LexemList lexems, LexemOperator.OperationType type, string[] operators, Action<LexemList> action, bool parseFromLeftToRight = false)
        {
            //обычный разбор выражения должен идти справа налево (<-)
            //например, попробуйте построить полиз для выражения: a-b+c или a/b*c
            //только разбирая одноуровневые операции справа налево получится корректный полиз!!!

            for (int i = parseFromLeftToRight ? 0 : lexems.Count - 1; 0 <= i && i < lexems.Count; i += (parseFromLeftToRight ? 1 : -1))
            {
                var lexem = lexems[i];

                if (lexem is LexemBracket)
                {
                    i = this.SkipBrackets(lexems, i, parseFromLeftToRight);
                }
                else if (lexem is LexemOperator)
                {
                    var op = lexem as LexemOperator;

                    if (op.OperatorType == type && (operators == null || operators.Contains(op.Text)))
                    {
                        this.OnOperator(op, lexems.Range(0, i), lexems.Range(i + 1, lexems.Count - i - 1));
                        return;
                    }
                }
            }

            if (action != null)
                action(lexems);
        }

        protected void Error(LexemList lexems)
        {
            throw new FormatException($"Expression syntax not recognized: {lexems}");
        }

        protected int SkipBrackets(LexemList lexems, int index, bool forward = true)
        {
            var bracket = lexems[index] as LexemBracket;
            if (bracket == null)
                return index;

            var result = index;

            if (bracket.Simetric != null)
                result = lexems.FindPosition(bracket.Simetric);

            if (forward && result <= index || !forward && result >= index)
                throw new FormatException($"Brackets balance is incorrect: {lexems}");

            return result;
        }
        #endregion

        #region Abstract
        protected abstract void OnSwitch(LexemSwitch lexem, LexemList condition, LexemList then, LexemList other);

        protected abstract void OnOperator(LexemOperator op, LexemList left, LexemList right);

        protected abstract void OnElement(Lexem lexem);

        protected abstract void OnElement(LexemList lexems);
        #endregion
    }
}
