using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public struct Bracket
    {
        private string _symbol;

        public string Symbol
        { get { return _symbol; } }

        public Bracket(string symbol)
        { _symbol = symbol; }

        public override string ToString()
        {
            return this.Symbol;
        }

        static public implicit operator string(Bracket bracket)
        { return bracket.Symbol; }

        static public implicit operator Bracket(string str)
        { return new Bracket(str); }

        static public implicit operator Bracket(char ch)
        { return new Bracket(ch.ToString()); }
    }

    public class BracketPair
    {
        private Bracket _open;
        private Bracket _close;

        public Bracket Open
        { get { return _open; } }

        public Bracket Close
        { get { return _close; } }

        public BracketPair(Bracket open, Bracket close)
        {
            _open = open;
            _close = close;
        }

        public bool Has(Bracket br)
        {
            return this.Open == br || this.Close == br;
        }
    }
}
