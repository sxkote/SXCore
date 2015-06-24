using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXLexemVoid : SXLexemValue
    {
        #region Variables
        protected object _value = null;
        #endregion

        #region Properties
        public override ValueType Type
        { get { return ValueType.Void; } }

        public object Value
        { get { return _value; } }
        #endregion

        #region Constructor
        public SXLexemVoid(object pointer)
        {
            _value = pointer;
        }
        #endregion

        #region Common
        public override string ToString()
        {
            return (this.Value == null) ? "null" : this.Value.ToString();
        }
        #endregion

        #region Statics
        public static SXLexemVoid Null
        { get { return new SXLexemVoid(null); } }

        new public static SXLexemVoid Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();
            if (!text.StartsWith("null", StringComparison.InvariantCultureIgnoreCase))
                return null;

            text = text.Crop(4);

            return SXLexemVoid.Null;
        }
        #endregion
    }
}
