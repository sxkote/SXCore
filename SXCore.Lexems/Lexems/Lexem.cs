using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXLexem
    {
        #region Statics
        static public SXLexem Parse(ref string text)
        {
            SXLexem result;
            if ((result = SXLexemKeyword.Parse(ref text)) != null)
                return result;
            if ((result = SXLexemValue.Parse(ref text)) != null)
                return result;
            if ((result = SXLexemSymbol.Parse(ref text)) != null)
                return result;
            if ((result = SXLexemFunction.Parse(ref text)) != null)
                return result;
            if ((result = SXLexemVariable.Parse(ref text)) != null)
                return result;
            return null;
        }

        public static List<string> Split(string text, char[] separators, BracketPair[] brackets = null)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            var result = new List<string>();

            var input = text;
            while (!String.IsNullOrEmpty(input))
            {
                var index = SXLexem.Find(input, separators, 0, brackets);

                var value = (index < 0) ? input : input.Substring(0, index);

                result.Add(value);

                input = input.Crop(value.Length + 1);
            }

            return result;
        }

        public static int Find(string text, char[] find, int index = 0, BracketPair[] brackets = null)
        {
            if (String.IsNullOrEmpty(text))
                return -1;

            var balancer = new SXBracketsStringBalancer(brackets);
            for (int i = index; i < text.Length; i++)
            {
                if (find.Contains(text[i]) && balancer.IsBalanced)
                    return i;

                balancer.Push(text[i].ToString());
            }

            return -1;
        }
        #endregion

        #region Operators
        public static implicit operator SXLexem(decimal value)
        { return (SXLexemNumber)value; }

        public static implicit operator SXLexem(double value)
        { return (SXLexemNumber)value; }

        public static implicit operator SXLexem(int value)
        { return (SXLexemNumber)value; }

        public static implicit operator SXLexem(bool value)
        { return (SXLexemBool)value; }

        public static implicit operator SXLexem(DateTime value)
        { return (SXLexemDate)value; }

        public static implicit operator SXLexem(TimeSpan value)
        { return (SXLexemSpan)value; }

        public static implicit operator SXLexem(string value)
        { return (SXLexemText)value; }
        #endregion
    }
}
