using SXCore.Lexems.Values;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SXCore.Lexems
{
    public abstract class Lexem
    {
        static public Lexem Parse(ref string text)
        {
            Lexem result;

            if ((result = LexemKeyword.Parse(ref text)) != null)
                return result;

            if ((result = LexemValue.Parse(ref text)) != null)
                return result;

            if ((result = LexemSymbol.Parse(ref text)) != null)
                return result;

            if ((result = LexemFunction.Parse(ref text)) != null)
                return result;

            if ((result = LexemVariable.Parse(ref text)) != null)
                return result;

            return null;
        }

        #region Operators
        public static implicit operator Lexem(decimal value)
        { return (LexemValueNumber)value; }

        public static implicit operator Lexem(double value)
        { return (LexemValueNumber)value; }

        public static implicit operator Lexem(int value)
        { return (LexemValueNumber)value; }

        public static implicit operator Lexem(bool value)
        { return (LexemValueBool)value; }

        public static implicit operator Lexem(DateTime value)
        { return (LexemValueDate)value; }

        public static implicit operator Lexem(TimeSpan value)
        { return (LexemValueSpan)value; }

        public static implicit operator Lexem(string value)
        { return (LexemValueText)value; }
        #endregion
    }

    public class LexemList : List<Lexem>
    {
        public LexemList() { }
        public LexemList(ICollection<Lexem> collecion) : base(collecion) { }
        public LexemList(string text)
        { this.Parse(text); }

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < this.Count; i++)
                if (this[i] is LexemValueNumber && ((LexemValueNumber)this[i]).Value < 0)
                    result += "(" + this[i].ToString() + ") ";
                else
                    result += this[i].ToString() + " ";
            return result.Trim();
        }

        public void Parse(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
                return;

            string input = text.Trim();

            var pairs = LexemBracket.Brackets.ToList();
            pairs.Add(new SymbolPair('?', ':'));

            var balancer = new BracketsBalanceLexemValidator(pairs.ToArray());

            while (!String.IsNullOrWhiteSpace(input))
            {
                var lexem = Lexem.Parse(ref input);

                if (lexem == null)
                    throw new ArgumentException(String.Format("Input string \"{0}\" can't be recognized as Lexems List", input ?? ""));

                if (lexem is LexemBracket || lexem is LexemSwitch)
                    balancer.Push(lexem);

                this.Add(lexem);

                input = String.IsNullOrWhiteSpace(input) ? null : input.Trim();
            }

            if (!balancer.IsBalanced)
                throw new ArgumentException(String.Format("Lexem List \"{0}\" is not balanced", text));
        }

        public LexemList Range(int index, int count)
        {
            return new LexemList(this.GetRange(index, count));
        }

        public int FindPosition(Lexem lexem)
        {
            return this.FindIndex(lex => ReferenceEquals(lexem, lex));
        }
    }
}
