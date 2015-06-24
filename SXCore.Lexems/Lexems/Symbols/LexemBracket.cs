using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXLexemBracket : SXLexemSymbol
    {
        static public BracketPair[] DefaultBrackets = { new BracketPair('(', ')'), new BracketPair('{', '}'), new BracketPair('[', ']') };

        #region Variables
        private SXLexemBracket _simetric = null;
        #endregion

        #region Properties
        public override SymbolType Type
        { get { return SymbolType.Bracket; } }

        public SXLexemBracket Simetric
        {
            get { return _simetric; }
            set
            {
                _simetric = value;
                if (_simetric != null && _simetric.Simetric != this)
                    _simetric.Simetric = this;
            }
        }
        #endregion

        #region Constructor
        public SXLexemBracket(string text)
        {
            _text = text.Trim();

            if (!DefaultBrackets.Any(br => br.Has(this.Text)))
                throw new FormatException("Unknown Bracket input");
        }
        #endregion

        #region Statics
        new static public SXLexemBracket Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            var input = text;
            var item = DefaultBrackets.SelectMany(p => new string[] { p.Open, p.Close }).FirstOrDefault(symb => input.StartsWith(symb));
            if (String.IsNullOrEmpty(item))
                return null;

            text = text.Crop(item.Length);

            return new SXLexemBracket(item);
        }
        #endregion
    }
}
