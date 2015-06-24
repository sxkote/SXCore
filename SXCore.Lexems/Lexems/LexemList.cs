using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SXCore.Lexems
{
    public class SXLexemList : List<SXLexem>
    {
        #region Constructors
        public SXLexemList() { }
        public SXLexemList(ICollection<SXLexem> collecion) : base(collecion) { }
        public SXLexemList(string text)
        { this.Parse(text); }
        #endregion

        #region Common
        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < this.Count; i++)
                if (this[i] is SXLexemNumber && ((SXLexemNumber)this[i]).Value < 0)
                    result += "(" + this[i].ToString() + ") ";
                else
                    result += this[i].ToString() + " ";
            return result.Trim();
        }
        #endregion

        #region Functions
        public void Parse(string text)
        {
            string analys = String.IsNullOrEmpty(text) ? null : text.Trim();

            var pairs = SXLexemBracket.DefaultBrackets.ToList();
            pairs.Add(new BracketPair('?', ':'));

            var balancer = new SXBracketsLexemBalancer(pairs.ToArray());

            while (!String.IsNullOrEmpty(analys))
            {
                var lexem = SXLexem.Parse(ref analys);

                if (lexem == null)
                    throw new ArgumentException(String.Format("Input string \"{0}\" can't be recognized as Lexems List", analys ?? ""));

                if (lexem is SXLexemBracket || lexem is SXLexemSwitch)
                    balancer.Push(lexem);

                this.Add(lexem);

                analys = String.IsNullOrEmpty(analys) ? null : analys.Trim();
            }

            if (!balancer.IsBalanced)
                throw new ArgumentException(String.Format("Lexem List \"{0}\" is not balanced", text));
        }

        public SXLexemList Range(int index, int count)
        {
            return new SXLexemList(this.GetRange(index, count));
        }

        public int FindPosition(SXLexem lexem)
        {
            return this.FindIndex(lex => (object)lexem == (object)lex);
        }
        #endregion
    }

   
}
