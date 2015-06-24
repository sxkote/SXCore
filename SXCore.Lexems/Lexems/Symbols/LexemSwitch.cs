using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXLexemSwitch : SXLexemSymbol
    {
        #region variables
        private SXLexemSwitch _simetric = null;
        //private SXLexem _begin = null;
        private SXLexem _end = null;
        #endregion

        #region Properties
        public override SymbolType Type
        { get { return SymbolType.Switch; } }

        public SXLexemSwitch Simetric
        {
            get { return _simetric; }
            set
            {
                _simetric = value;
                if (_simetric != null && _simetric.Simetric != this)
                    _simetric.Simetric = this;
            }
        }

        //public SXLexem Begin
        //{
        //    get { return _begin; }
        //    set
        //    {
        //        _begin = value;
        //        if (this.Simetric != null && this.Simetric.Begin != value)
        //            this.Simetric.Begin = value;
        //    }
        //}

        public SXLexem End
        {
            get { return _end; }
            set
            {
                _end = value;
                if (this.Simetric != null && this.Simetric.End != value)
                    this.Simetric.End = value;
            }
        }

        //public SXLexemSwitch Question
        //{ get { return (this.Text == "?") ? this : this.Simetric; } }

        //public SXLexemSwitch Answer
        //{ get { return (this.Text == ":") ? this : this.Simetric; } }
        #endregion

        #region Constructor
        public SXLexemSwitch(string text)
        {
            _text = text.Trim();

            if (this.Text != "?" && this.Text != ":")
                throw new FormatException("Unknown Switch input");
        }
        #endregion

        #region Statics
        new static public SXLexemSwitch Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();
            if (!text.StartsWith("?") && !text.StartsWith(":"))
                return null;

            var result = new SXLexemSwitch(text[0].ToString());

            text = text.Crop(1);

            return result;
        }
        #endregion
    }
}
