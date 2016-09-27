using System;
using System.Collections.Generic;
using System.Linq;

namespace SXCore.Lexems
{
    static public class Extensions
    {
        static public string Crop(this string text, int length)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            if (text.Length <= length)
                return null;

            return text.Substring(length);
        }

        static public bool Like(this string text, string pattern)
        {
            pattern = System.Text.RegularExpressions.Regex.Escape(pattern);

            pattern = pattern.Replace("%", ".*?").Replace("_", ".");
            pattern = pattern.Replace(@"\[", "[").Replace(@"\]", "]").Replace(@"\^", "^");

            return System.Text.RegularExpressions.Regex.IsMatch(text, pattern);
        }

        static public List<string> Split(this string text, char[] separators, SymbolPair[] brackets = null)
        {
            if (String.IsNullOrWhiteSpace(text))
                return null;

            var result = new List<string>();

            var input = text;
            while (!String.IsNullOrWhiteSpace(input))
            {
                var index = input.Find(separators, 0, brackets);

                var value = (index < 0) ? input : input.Substring(0, index);

                result.Add(value);

                input = input.Crop(value.Length + 1);
            }

            return result;
        }

        static public int Find(this string text, char[] find, int index = 0, SymbolPair[] brackets = null)
        {
            if (String.IsNullOrWhiteSpace(text))
                return -1;

            var balancer = new BracketsBalanceStringValidator(brackets);
            for (int i = index; i < text.Length; i++)
            {
                if (find.Contains(text[i]) && balancer.IsBalanced)
                    return i;

                balancer.Push(text[i].ToString());
            }

            return -1;
        }
    }

    public struct Symbol
    {
        public string Text { get; private set; }

        public Symbol(string text)
        {
            if (String.IsNullOrEmpty(text))
                throw new ArgumentException("Symbol can't be null or empty");

            this.Text = text;
        }

        public override string ToString()
        { return this.Text; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (obj is Symbol)
                return this.Text.Equals(((Symbol)obj).Text);

            return this.Text.Equals(obj.ToString());
        }

        public override int GetHashCode()
        {
            return this.Text.GetHashCode();
        }

        static public implicit operator string (Symbol symbol)
        { return symbol.Text; }

        static public implicit operator Symbol(string str)
        { return new Symbol(str); }

        static public implicit operator Symbol(char ch)
        { return new Symbol(ch.ToString()); }
    }

    public class SymbolPair
    {
        public Symbol Open { get; private set; }
        public Symbol Close { get; private set; }

        public SymbolPair(Symbol open, Symbol close)
        {
            this.Open = open;
            this.Close = close;
        }

        public bool Contains(Symbol symbol)
        {
            return this.Open == symbol || this.Close == symbol;
        }
    }
}
