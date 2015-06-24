using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Lexems
{
    public class SXLexemVariable : SXLexem
    {
        #region Variables
        protected string _name = "";
        protected SXLexemValue _value = 0;
        #endregion

        #region Properties
        public string Name
        { get { return _name; } }

        public SXLexemValue Value
        {
            get { return _value; }
            set { _value = value; }
        }
        #endregion

        #region Constructors
        public SXLexemVariable(string name, SXLexemValue value = null)
        {
            _name = name;

            if (!SXLexemVariable.CheckName(this.Name))
                throw new FormatException(String.Format("Variable Name {0} is incorrect", this.Name ?? ""));

            _value = value;
        }

        protected SXLexemVariable(SXLexemValue value)
        {
            _value = value;
        }
        #endregion

        #region Common
        public override string ToString()
        { 
            //return String.IsNullOrEmpty(this.Name) ? this.Value.ToString() : this.Name; 
            return this.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj is SXLexemValue)
            {
                return this.Value.Equals(obj);
            }

            if (obj is SXLexemVariable)
            {
                return this.Value.Equals((obj as SXLexemVariable).Value);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Value == null ? 0 : this.Value.GetHashCode();
        }
        #endregion

        #region Calculations
        public SXLexemVariable Execute(SXLexem lexem, IEnvironment environment = null)
        {
            if (this.Value == null)
                return null;

            return this.Value.Execute(lexem, environment);
        }
        #endregion

        #region Operators
        public static implicit operator SXLexemVariable(SXLexemValue value)
        { return new SXLexemVariable(value); }

        public static implicit operator SXLexemVariable(decimal value)
        { return new SXLexemVariable(value); }

        public static implicit operator SXLexemVariable(double value)
        { return new SXLexemVariable(value); }

        public static implicit operator SXLexemVariable(int value)
        { return new SXLexemVariable(value); }

        public static implicit operator SXLexemVariable(bool value)
        { return new SXLexemVariable(value); }

        public static implicit operator SXLexemVariable(DateTime value)
        { return new SXLexemVariable(value); }

        public static implicit operator SXLexemVariable(TimeSpan value)
        { return new SXLexemVariable(value); }

        public static implicit operator SXLexemVariable(string value)
        { return new SXLexemVariable(value); }
        #endregion

        #region Statics
        static public bool CheckName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return false;

            for (int i = 0; i < name.Length; i++)
            {
                if (i == 0 && (name[i] == '@' || name[i] == '$'))
                    continue;

                if (!Char.IsLetterOrDigit(name[i]) && name[i] != '_')
                    return false;
            }

            return true;
        }

        new public static SXLexemVariable Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            string input = "";
            for (int i = 0; i < text.Length; i++)
                if ((i == 0 && (text[i] == '@' || text[i] == '$')) || Char.IsLetterOrDigit(text[i]) || text[i] == '_')
                    input += text[i];
                else break;

            try
            {
                var result = new SXLexemVariable(input);

                text = text.Crop(input.Length);

                return result;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
