using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXLexemKeyword : SXLexem
    {
        #region Keywords
        static public string[] Keywords = 
        {
            "text", "string", 
            "number", "double", "decimal", "float", "money", 
            "int","bigint", "long",
            "bit", "bool", "boolean",
            "date","datetime", "time",
            "span", "timespan",
            "complex","void",
            "public", "protected", "private",
            "if", "else", "for", "while", "do", "begin", "end", "return", "command", "//",
            "class", "variable", "var", "function", "func"
        };
        #endregion

        #region Variables
        protected string _text = "";
        #endregion

        #region Properties
        public string Text
        { get { return _text; } }
        #endregion

        #region Constructor
        public SXLexemKeyword(string text)
        {
            _text = text.Trim().ToLower();

            if (!Keywords.Contains(this.Text))
                throw new FormatException("Unknown Keyword input");
        }
        #endregion

        #region Common
        public override string ToString()
        { return this.Text; }
        #endregion

        #region Statics
        new static public SXLexemKeyword Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            string word = "";
            for (int i = 0; i < text.Length; i++)
                if (Char.IsLetter(text[i]) || text[i] == '_')
                    word += text[i];
                else break;

            if (text[0] == '/' && text[1] == '/') word = "//";


            if (Keywords.Any(k => k == word.ToLower()))
            {
                text = text.Crop(word.Length);

                return new SXLexemKeyword(word);
            }

            return null;
        }
        #endregion
    }
}