using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class LexemKeyword : Lexem
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

        public string Text { get; private set; }

        public LexemKeyword(string text)
        {
            if (String.IsNullOrEmpty(text))
                throw new ArgumentException("Keyword can't be null or empty");

            this.Text = text.Trim().ToLower();

            if (!Keywords.Contains(this.Text))
                throw new FormatException("Unknown Keyword");
        }

        public override string ToString()
        { return this.Text; }

        new static public LexemKeyword Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            Match match = Regex.Match(text, @"^(\w+|\/\/)");
            if (!match.Success || !Keywords.Any(k => k.Equals(match.Value, StringComparison.InvariantCultureIgnoreCase)))
                return null;

            text = text.Crop(match.Value.Length);

            return new LexemKeyword(match.Value);
        }
    }
}