using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXLexemComma : SXLexemSymbol
    {
        static public string[] Commas = { ",", ";" };

        #region Properties
        public override SymbolType Type
        { get { return SymbolType.Comma; } }
        #endregion

        #region Constructor
        public SXLexemComma(string text)
        {
            _text = text.Trim();

            if (!Commas.Contains(this.Text))
                throw new FormatException("Unknown Comma input");
        }
        #endregion

        #region Statics
        new static public SXLexemComma Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            var input = text;
            var item = Commas.FirstOrDefault(o => input.StartsWith(o));
            if (String.IsNullOrEmpty(item))
                return null;

            text = text.Crop(item.Length);

            return new SXLexemComma(item);
        }
        #endregion
    }
}
