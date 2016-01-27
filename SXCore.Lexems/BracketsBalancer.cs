using SXCore.Lexems.Values;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SXCore.Lexems
{
    internal interface IBracketsBalancer<T>
    {
        bool IsBalanced { get; }
        void Push(T arg);
    }

    internal class BracketsStringBalancer : IBracketsBalancer<string>
    {
        public List<SymbolPair> Pairs { get; private set; }
        public List<string> Quotes { get; private set; }

        protected string _currentQuote = "";
        protected Stack<string> _balance;

        public bool IsBalanced
        { get { return _balance.Count <= 0 && String.IsNullOrEmpty(_currentQuote); } }

        public BracketsStringBalancer(SymbolPair[] pairs, string[] quotes)
        {
            this.Pairs = pairs == null ? new List<SymbolPair>() : new List<SymbolPair>(pairs);
            this.Quotes = quotes == null ? new List<string>() : new List<string>(quotes);

            _currentQuote = "";
            _balance = new Stack<string>();
        }

        public BracketsStringBalancer(params SymbolPair[] pairs)
            : this(pairs, LexemValueText.Quotes) { }

        public void Push(string value)
        {
            // we are already in the quoted text part
            if (!String.IsNullOrEmpty(_currentQuote))
            { 
                if (value == _currentQuote)
                    _currentQuote = "";

                return;
            }

            // begin of the quoted text part
            if (this.Quotes.Contains(value))
            {
                _currentQuote = value;
                return;
            }

            // if the bracket is pushed
            var pair = this.Pairs.FirstOrDefault(p => p.Contains(value));
            if (pair == null)
                return;
            
            // try to close open bracket
            if (value == pair.Close && _balance.Count > 0 && _balance.Peek() == pair.Open)
                _balance.Pop();
            else
                _balance.Push(value);
        }
    }

    internal class BracketsLexemBalancer : IBracketsBalancer<Lexem>
    {
        public List<SymbolPair> Pairs {get; private set;}

        protected Stack<Lexem> _balance;

        public bool IsBalanced
        { get { return _balance.Count <= 0; } }

        public BracketsLexemBalancer(params SymbolPair[] pairs)
        {
            this.Pairs = pairs == null ? new List<SymbolPair>() : new List<SymbolPair>(pairs);

            _balance = new Stack<Lexem>();
        }

        public void Push(Lexem value)
        {
            if (value == null)
                return;

            // if the bracket is pushed
            var pair = Pairs.FirstOrDefault(p => p.Contains(value.ToString()));
            if (pair == null)
                return;

            // try to close open bracket
            if (value.ToString() == pair.Close && _balance.Count > 0 && _balance.Peek().ToString() == pair.Open)
            {
                var open = _balance.Pop();

                if (open is LexemBracket && value is LexemBracket)
                    ((LexemBracket)open).Simetric = (LexemBracket)value;

                if (open is LexemSwitch && value is LexemSwitch)
                    ((LexemSwitch)open).Simetric = (LexemSwitch)value;
            }
            else
                _balance.Push(value);
        }
    }
}
