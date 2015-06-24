using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public abstract class SXAnalyser
    {
        #region Analys
        protected virtual void Analys(SXLexemList lexems)
        { this.AnalysExpression(lexems); }

        protected virtual void AnalysExpression(SXLexemList lexems)
        {
            for (int i = 0; i < lexems.Count; i++)
            {
                var lexem = lexems[i];

                if (lexem is SXLexemBracket)
                {
                    i = this.SkipBrackets(lexems, i);
                }
                else if (lexem is SXLexemSwitch && lexem.ToString() == "?")
                {
                    var sw = lexem as SXLexemSwitch;

                    var simetric = sw.Simetric;
                    if (simetric == null)
                        throw new FormatException("Switch should have Simetric Else path");

                    int index = lexems.FindPosition(simetric);
                    if (index <= i)
                        throw new FormatException("Switch balance is incorrect");

                    var condition = lexems.Range(0, i);
                    var then = lexems.Range(i + 1, index - i - 1);
                    var other = lexems.Range(index + 1, lexems.Count - index - 1);

                    this.OnSwitch(sw, condition, then, other);

                    return;
                }
            }

            this.AnalysLogical(lexems);
        }

        protected void AnalysLogical(SXLexemList lexems)
        { this.AnalysLogicalOr(lexems); }

        protected virtual void AnalysLogicalOr(SXLexemList lexems)
        {
            this.Scan(lexems, SXLexemOperator.OperationType.Logical, new string[] { "||" }, this.AnalysLogicalAnd);
        }

        protected virtual void AnalysLogicalAnd(SXLexemList lexems)
        {
            this.Scan(lexems, SXLexemOperator.OperationType.Logical, new string[] { "&&" }, this.AnalysComparison);
        }

        protected virtual void AnalysComparison(SXLexemList lexems)
        {
            this.Scan(lexems, SXLexemOperator.OperationType.Comparison, null, this.AnalysArithmetic);
        }

        protected void AnalysArithmetic(SXLexemList lexems)
        { this.AnalysArithmeticAddition(lexems); }

        protected virtual void AnalysArithmeticAddition(SXLexemList lexems)
        {
            this.Scan(lexems, SXLexemOperator.OperationType.Arithmetic, new string[] { "+", "-" }, this.AnalysArithmeticMultiplication);
        }

        protected virtual void AnalysArithmeticMultiplication(SXLexemList lexems)
        {
            this.Scan(lexems, SXLexemOperator.OperationType.Arithmetic, new string[] { "*", "/" }, this.AnalysElement);
        }

        protected virtual void AnalysElement(SXLexemList lexems)
        {
            if (lexems == null || lexems.Count <= 0)
                throw new FormatException(String.Format("Expression Element is empty"));

            if (lexems.Count == 1)
            {
                this.OnElement(lexems[0]);
                return;
            }

            if (lexems[0] is SXLexemBracket && lexems.FindPosition((lexems[0] as SXLexemBracket).Simetric) == lexems.Count - 1)
            {
                this.OnElement(lexems);
                return;
            }

            this.Scan(lexems, SXLexemOperator.OperationType.Code, new string[] { "->", "." }, this.Error);
        }
        #endregion

        #region Functions
        protected void Scan(SXLexemList lexems, SXLexemOperator.OperationType type, string[] operators, Action<SXLexemList> action, bool forward = false)
        {
            //обычный разбор выражения должен идти справа налево (<-)
            //например, попробуйте построить полиз для выражения: a-b+c или a/b*c
            //только разбирая одноуровневые операции справа налево получится корректный полиз!!!

            for (int i = forward ? 0 : lexems.Count - 1; 0 <= i && i < lexems.Count; i += (forward ? 1 : -1))
            {
                var lexem = lexems[i];

                if (lexem is SXLexemBracket)
                {
                    i = this.SkipBrackets(lexems, i, forward);
                }
                else if (lexem is SXLexemOperator)
                {
                    var op = lexem as SXLexemOperator;

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

        protected void Error(SXLexemList lexems)
        {
            throw new FormatException(String.Format("Expression not recognized: {0}", lexems.ToString()));
        }

        protected int SkipBrackets(SXLexemList lexems, int index, bool forward = true)
        {
            var bracket = lexems[index] as SXLexemBracket;
            if (bracket == null)
                return index;

            var result = index;

            if (bracket.Simetric != null)
                result = lexems.FindPosition(bracket.Simetric);

            if (forward && result <= index || !forward && result >= index)
                throw new FormatException(String.Format("Bracket balance is incorrect: {0}", lexems));

            return result;
        }
        #endregion

        #region Abstract
        protected abstract void OnSwitch(SXLexemSwitch lexem, SXLexemList condition, SXLexemList then, SXLexemList other);

        protected abstract void OnOperator(SXLexemOperator or, SXLexemList left, SXLexemList right);

        protected abstract void OnElement(SXLexem lexem);

        protected abstract void OnElement(SXLexemList lexems);
        #endregion
    }
}
