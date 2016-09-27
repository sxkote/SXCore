using SXCore.Lexems.Values;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SXCore.Lexems
{
    internal interface IBracketsBalanceValidator<T>
    {
        /// <summary>
        /// Is the input is balanced
        /// </summary>
        bool IsBalanced { get; }

        /// <summary>
        /// Push nex input item in validator
        /// </summary>
        /// <param name="arg">Next item of the input</param>
        void Push(T arg);
    }

    internal class BracketsBalanceStringValidator : IBracketsBalanceValidator<string>
    {
        /// <summary>
        /// Collection of Brackets Pairs used in validation
        /// </summary>
        public List<SymbolPair> Pairs { get; private set; }

        /// <summary>
        /// Collection of Quotes used in validation
        /// </summary>
        public List<string> Quotes { get; private set; }

        /// <summary>
        /// Current Quote in the input
        /// </summary>
        protected string _currentQuote = "";

        /// <summary>
        /// Stack of Brackets in the input
        /// </summary>
        protected Stack<string> _balance;

        /// <summary>
        /// Is the input is balanced
        /// </summary>
        public bool IsBalanced
        { get { return _balance.Count <= 0 && String.IsNullOrEmpty(_currentQuote); } }

        public BracketsBalanceStringValidator(SymbolPair[] pairs, string[] quotes)
        {
            this.Pairs = pairs == null ? new List<SymbolPair>() : new List<SymbolPair>(pairs);
            this.Quotes = quotes == null ? new List<string>() : new List<string>(quotes);

            _currentQuote = "";
            _balance = new Stack<string>();
        }

        public BracketsBalanceStringValidator(params SymbolPair[] pairs)
            : this(pairs, LexemValueText.Quotes) { }

        /// <summary>
        /// Push next item of the input
        /// </summary>
        /// <param name="value">Next item</param>
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

    internal class BracketsBalanceLexemValidator : IBracketsBalanceValidator<Lexem>
    {
        /// <summary>
        /// Collection of Brackets Pairs used in validation
        /// </summary>
        public List<SymbolPair> Pairs {get; private set;}

        /// <summary>
        /// Stack of Brackets in the input
        /// </summary>
        protected Stack<Lexem> _balance;

        /// <summary>
        /// Is the input is balanced
        /// </summary>
        public bool IsBalanced
        { get { return _balance.Count <= 0; } }

        public BracketsBalanceLexemValidator(params SymbolPair[] pairs)
        {
            this.Pairs = pairs == null ? new List<SymbolPair>() : new List<SymbolPair>(pairs);

            _balance = new Stack<Lexem>();
        }


        /// <summary>
        /// Push next item of the input
        /// </summary>
        /// <param name="value">Next item</param>
        public void Push(Lexem value)
        {
            if (value == null)
                return;

            // if the bracket is pushed
            var pair = this.Pairs.FirstOrDefault(p => p.Contains(value.ToString()));
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
