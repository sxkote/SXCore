using System;
using System.Collections.Generic;
using System.Linq;

namespace SXCore.Lexems
{
    public interface ILexemEnvironment
    {
        ICollection<LexemVariable> Variables { get; }

        void Clear();

        LexemVariable Get(string name);
        LexemVariable Set(string name, LexemValue value);

        LexemVariable Calculate(LexemFunction function);
    }

    public class LexemEnvironment : ILexemEnvironment
    {
        public delegate LexemVariable OnFunctionExecuting(LexemFunction function);

        protected List<LexemVariable> _variables = new List<LexemVariable>();
        protected OnFunctionExecuting onFunctionExecuting = null;

        public ICollection<LexemVariable> Variables
        { get { return _variables.AsReadOnly(); } }

        public OnFunctionExecuting FunctionExecuting
        {
            get { return onFunctionExecuting; }
            set { onFunctionExecuting = value; }
        }

        public LexemVariable this[string name]
        {
            get
            {
                return this.Get(name);
            }
            set
            {
                if (value == null || String.IsNullOrEmpty(name))
                    return;

                this.Set(name, value.Value);
            }
        }

        public LexemEnvironment() { }

        public LexemEnvironment(IEnumerable<LexemVariable> variables)
        { _variables = variables.ToList(); }

        public void Clear()
        {
            _variables.Clear();
        }

        public LexemVariable Get(string name)
        {
            return this.Variables.FirstOrDefault(v => v.Name.Equals(name));
        }

        public LexemVariable Set(string name, LexemValue value)
        {
            if (value == null || String.IsNullOrEmpty(name))
                return null;

            var variable = this.Get(name);

            if (variable == null)
                return this.Add(name, value);

            variable.Value = value;

            return variable;
        }

        public LexemVariable Add(string name, LexemValue value)
        {
            var variable = new LexemVariable(name, value);

            _variables.Add(variable);

            return variable;
        }

        public virtual LexemVariable Calculate(LexemFunction function)
        {
            if (this.FunctionExecuting != null)
                return this.FunctionExecuting(function);

            return null;
        }
    }
}
