using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXLexemSymbol : SXLexem
    {
        public enum SymbolType { None, Comma, Bracket, Operator, Switch };

        #region Variables
        protected string _text = "";
        #endregion

        #region Properties
        public virtual SymbolType Type
        { get { return SymbolType.None; } }

        public string Text
        { get { return _text; } }
        #endregion

        #region Common
        public override string ToString()
        { return this.Text; }
        #endregion

        #region Statics
        new static public SXLexemSymbol Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            SXLexemSymbol result = null;
            if ((result = SXLexemOperator.Parse(ref text)) != null)
                return result;
            if ((result = SXLexemBracket.Parse(ref text)) != null)
                return result;
            if ((result = SXLexemComma.Parse(ref text)) != null)
                return result;
            if ((result = SXLexemSwitch.Parse(ref text)) != null)
                return result;
            return null;
        }
        #endregion
    }
}
