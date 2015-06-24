using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    internal interface IBracketsBalancer<T>
    {
        bool IsBalanced { get; }
        void Push(T arg);
    }

    internal class SXBracketsStringBalancer : IBracketsBalancer<string>
    {
        public static string[] DefaultQuotes = { "\"", "'" };

        protected List<BracketPair> _pairs;
        protected List<string> _quotes;

        protected string _quote = "";
        protected Stack<string> _balance;

        public bool IsBalanced
        { get { return _balance.Count <= 0 && String.IsNullOrEmpty(_quote); } }

        public SXBracketsStringBalancer(BracketPair[] pairs, string[] quotes)
        {
            _pairs = pairs == null ? new List<BracketPair>() : new List<BracketPair>(pairs);
            _quotes = quotes == null ? new List<string>() : new List<string>(quotes);

            _quote = "";
            _balance = new Stack<string>();
        }

        public SXBracketsStringBalancer(params BracketPair[] pairs)
            : this(pairs, DefaultQuotes) { }

        public void Push(string value)
        {
            // we are already in the quoted text part
            if (!String.IsNullOrEmpty(_quote))
            { 
                if (value == _quote)
                    _quote = "";

                return;
            }

            // begin of the quoted text part
            if (_quotes.Contains(value))
            {
                _quote = value;
                return;
            }

            // if the bracket is pushed
            var pair = _pairs.FirstOrDefault(p => p.Has(value));
            if (pair == null)
                return;
            
            // try to close open bracket
            if (value == pair.Close && _balance.Count > 0 && _balance.Peek() == pair.Open)
                _balance.Pop();
            else
                _balance.Push(value);
        }
    }

    internal class SXBracketsLexemBalancer : IBracketsBalancer<SXLexem>
    {
        protected List<BracketPair> _pairs;
        protected Stack<SXLexem> _balance;

        public bool IsBalanced
        { get { return _balance.Count <= 0; } }

        public SXBracketsLexemBalancer(params BracketPair[] pairs)
        {
            _pairs = pairs == null ? new List<BracketPair>() : new List<BracketPair>(pairs);
            _balance = new Stack<SXLexem>();
        }

        public void Push(SXLexem value)
        {
            if (value == null)
                return;

            // if the bracket is pushed
            var pair = _pairs.FirstOrDefault(p => p.Has(value.ToString()));
            if (pair == null)
                return;

            // try to close open bracket
            if (value.ToString() == pair.Close && _balance.Count > 0 && _balance.Peek().ToString() == pair.Open)
            {
                var open = _balance.Pop();

                if (open is SXLexemBracket && value is SXLexemBracket)
                    ((SXLexemBracket)open).Simetric = (SXLexemBracket)value;

                if (open is SXLexemSwitch && value is SXLexemSwitch)
                    ((SXLexemSwitch)open).Simetric = (SXLexemSwitch)value;
            }
            else
                _balance.Push(value);
        }
    }
}
