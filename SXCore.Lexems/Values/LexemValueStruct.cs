using System;
using System.Collections.Generic;
using System.Linq;

namespace SXCore.Lexems.Values
{
    public class LexemValueStruct : LexemValue
    {
        protected List<LexemVariable> _members = new List<LexemVariable>();

        public IList<LexemVariable> Members
        { get { return _members.AsReadOnly(); } }

        public LexemVariable this[string name]
        {
            get
            {
                return this.Members.FirstOrDefault(m => m.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            }
            set
            {
                if (value == null)
                    return;

                var member = this[name];

                if (member != null)
                    member.Value = value.Value;
                else
                    this.Members.Add(value);
            }
        }

        public LexemValueStruct(string text)
        {
            string input = String.IsNullOrEmpty(text) ? null : text.Trim();
            if (String.IsNullOrEmpty(input) || input[0] != '{' || input[input.Length - 1] != '}')
                throw new FormatException("Struct format is incorrect: no brackets");

            // get input without brackets
            input = input.Substring(1, input.Length - 2).Trim();

            // split the input text into members inputs
            var values = input.Split(new char[] { ',' }, new SymbolPair[] { new SymbolPair('{', '}') });
            if (values == null || values.Count <= 0)
                throw new FormatException("Struct format is incorrect: no members");

            // get members from from inputs
            foreach (var value in values)
            {
                var member = LexemValueStruct.ParseMember(value);
                if (member == null)
                    throw new FormatException("Struct format is incorrect: can't parse member");

                _members.Add(member);
            }
        }

        public LexemValueStruct(IEnumerable<LexemVariable> members)
        {
            _members = members == null ? new List<LexemVariable>() : members.ToList();
        }

        public override string ToString()
        {
            if (this.Members == null)
                return "{}";

            var records = this.Members.Select(m => String.Format("\"{0}\":{1}", m.Name, m.Value.ToString()));

            return "{" + String.Join(",", records) + "}";
        }

        public override LexemVariable Execute(Lexem lexem, ILexemEnvironment environment = null)
        {
            if (lexem == null)
                throw new InvalidOperationException("Can't execute null lexem on Value");

            if (lexem is LexemVariable)
                return this[((LexemVariable)lexem).Name];

            return base.Execute(lexem, environment);
        }

        new public static LexemValueStruct Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            // trim the text and check possibility
            text = text.Trim();
            if (text.Length < 2 || text[0] != '{')
                return null;

            // find the end of the lexem (find closing bracket '}')
            int index = text.Find(new char[] { '}' }, 1, new SymbolPair[] { new SymbolPair('{', '}') });
            if (index <= 0)
                return null;

            // get the lexem input string
            var input = text.Substring(0, index + 1).Trim();
            if (String.IsNullOrEmpty(input) || input.Length < 2 || input[0] != '{' || input[input.Length - 1] != '}')
                return null;

            try
            {
                var result = new LexemValueStruct(input);

                text = text.Crop(index + 1);

                return result;
            }
            catch
            {
                return null;
            }
        }

        public static LexemVariable ParseMember(string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            var index = text.IndexOf(":");
            if (index <= 0)
                return null;

            // parse name of the member
            var name = text.Substring(0, index);
            if (name.Length >= 2 && (name[0] == '"' && name[name.Length - 1] == '"' || name[0] == '\'' && name[name.Length - 1] == '\''))
                name = name.Substring(1, name.Length - 2);

            // parse value of the member
            var value = LexemValue.ParseExact(text.Substring(index + 1));
            if (value == null)
                return null;

            return new LexemVariable(name, value);
        }
    }
}
