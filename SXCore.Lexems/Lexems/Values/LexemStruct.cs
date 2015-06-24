using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXLexemStruct : SXLexemValue
    {
        #region Variables
        protected List<SXLexemVariable> _members = new List<SXLexemVariable>();
        #endregion

        #region Properties
        public override ValueType Type
        { get { return ValueType.Struct; } }

        public IList<SXLexemVariable> Members
        { get { return _members.AsReadOnly(); } }

        public SXLexemVariable this[string name]
        {
            get
            {
                return this.Members.FirstOrDefault(m => m.Name == name);
            }
            set
            {
                if (value == null)
                    return;

                var member = this.Members.FirstOrDefault(m => m.Name == name);

                if (member != null)
                    member.Value = value.Value;
                else
                    this.Members.Add(value);
            }
        }
        #endregion

        #region Constructors
        public SXLexemStruct(string text)
        {
            string input = String.IsNullOrEmpty(text) ? null : text.Trim();
            if (String.IsNullOrEmpty(input) || input[0] != '{' || input[input.Length - 1] != '}')
                throw new FormatException("Struct Value input is empty");

            // split the input text into members inputs
            var values = SXLexem.Split(input.Substring(1, input.Length - 2).Trim(), new char[] { ',' }, new BracketPair[] { new BracketPair('{', '}') });
            if (values == null || values.Count <= 0)
                throw new FormatException("Struct Value format is wrong");

            // get members from from inputs
            foreach (var value in values)
            {
                var member = SXLexemStruct.ParseMember(value);
                if (member == null)
                    throw new FormatException("Struct Value format is wrong");

                _members.Add(member);
            }
        }

        public SXLexemStruct(IEnumerable<SXLexemVariable> members)
        {
            _members = members == null ? new List<SXLexemVariable>() : members.ToList();
        }
        #endregion

        #region Common
        public override string ToString()
        {
            if (this.Members == null)
                return "{}";

            var records = this.Members.Select(m => String.Format("\"{0}\":{1}", m.Name, m.Value.ToString()));

            return "{" + String.Join(",", records)+ "}";
        }
        #endregion

        #region Calculations
        public override SXLexemVariable Execute(SXLexem lexem, IEnvironment environment = null)
        {
            if (lexem == null) return null;

            if (lexem is SXLexemVariable)
                return this[((SXLexemVariable)lexem).Name];

            return base.Execute(lexem, environment);
        }
        #endregion

        #region Statics
       new  public static SXLexemStruct Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            // trim the text and check possibility
            text = text.Trim();
            if (text.Length < 2 || text[0] != '{')
                return null;

            // find the end of the lexem (find closing bracket '}')
            int index = SXLexem.Find(text, new char[] { '}' }, 1, new BracketPair[] { new BracketPair('{', '}') });
            if (index <= 0)
                return null;

            // get the lexem input string
            var input = text.Substring(0, index + 1).Trim();
            if (String.IsNullOrEmpty(input) || input.Length < 2 || input[0] != '{' || input[input.Length - 1] != '}')
                return null;

            try
            {
                var result = new SXLexemStruct(input);

                text = text.Crop(index + 1);

                return result;
            }
            catch
            {
                return null;
            }
        }

        public static SXLexemVariable ParseMember(string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            var index = text.IndexOf(":");
            if (index <= 0)
                return null;

            // parse name of the member
            var name = text.Substring(0, index);
            if (name.Length >= 2 && (name[0] == '"' && name[name.Length - 1] == '"' ||  name[0] == '\'' && name[name.Length - 1] == '\''))
                name = name.Substring(1, name.Length - 2);

            // parse value of the member
            var value = SXLexemValue.ParseExact(text.Substring(index + 1));
            if (value == null)
                return null;

            return new SXLexemVariable(name, value);
        }
        #endregion
    }
}
