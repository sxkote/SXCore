using System;
using System.Text.RegularExpressions;

namespace SXCore.Lexems
{
    public class LexemVariable : Lexem
    {
        public const string VariableNamePattern = @"[\@\$]?\w+";

        public string Name { get; private set; }
        public LexemValue Value { get; set; }

        private LexemVariable() { }

        public LexemVariable(string name, LexemValue value = null)
            :this()
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("Variable Name can't be null or empty");

            this.Name = name;

            if (!CheckVariableName(this.Name))
                throw new FormatException(String.Format($"Variable Name {this.Name} is incorrect"));

            this.Value = value;
        }

        public override string ToString()
        { return this.Name; }

        public override bool Equals(object obj)
        {
            if (obj is LexemValue)
            {
                return this.Value.Equals(obj);
            }

            if (obj is LexemVariable)
            {
                return this.Value.Equals((obj as LexemVariable).Value);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Value == null ? 0 : this.Value.GetHashCode();
        }

        public LexemVariable Execute(Lexem lexem, ILexemEnvironment environment = null)
        {
            if (this.Value == null)
                return null;

            return this.Value.Execute(lexem, environment);
        }

        static public bool CheckVariableName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return false;

            return Regex.IsMatch(name, $"^({VariableNamePattern})$");
        }

        new public static LexemVariable Parse(ref string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            text = text.Trim();

            Match match = Regex.Match(text, $"^{VariableNamePattern}");
            if (!match.Success || String.IsNullOrEmpty(match.Value))
                return null;

            text = text.Crop(match.Value.Length);

            return new LexemVariable(match.Value);
        }

        #region Operators
        public static implicit operator LexemVariable(LexemValue value)
        { return new LexemVariable() { Value = value }; }

        public static implicit operator LexemVariable(decimal value)
        { return new LexemVariable() { Value = value }; }

        public static implicit operator LexemVariable(double value)
        { return new LexemVariable() { Value = value }; }

        public static implicit operator LexemVariable(int value)
        { return new LexemVariable() { Value = value }; }

        public static implicit operator LexemVariable(bool value)
        { return new LexemVariable() { Value = value }; }

        public static implicit operator LexemVariable(DateTime value)
        { return new LexemVariable() { Value = value }; }

        public static implicit operator LexemVariable(TimeSpan value)
        { return new LexemVariable() { Value = value }; }

        public static implicit operator LexemVariable(string value)
        { return new LexemVariable() { Value = value }; }
        #endregion
    }
}
