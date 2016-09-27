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
        LexemVariable Execute(LexemValue argument, Lexem lexem);
    }

    public class LexemEnvironment : ILexemEnvironment
    {
        public delegate LexemVariable OnFunctionCalculationDelegate(LexemFunction function);
        public delegate LexemVariable OnLexemExecutionDelegate(LexemValue argument, Lexem lexem);

        protected List<LexemVariable> _variables = new List<LexemVariable>();

        protected OnFunctionCalculationDelegate _onFunctionCalculation = null;
        protected OnLexemExecutionDelegate _onLexemExecution = null;

        public ICollection<LexemVariable> Variables
        { get { return _variables.AsReadOnly(); } }

        public OnFunctionCalculationDelegate OnFunctionCalculation
        {
            get { return _onFunctionCalculation; }
            set { _onFunctionCalculation = value; }
        }

        public OnLexemExecutionDelegate OnLexemExecution
        {
            get { return _onLexemExecution; }
            set { _onLexemExecution = value; }
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
            if (this.OnFunctionCalculation != null)
                return this.OnFunctionCalculation(function);

            return null;
        }

        public virtual LexemVariable Execute(LexemValue argument, Lexem lexem)
        {
            if (this.OnLexemExecution != null)
                return this.OnLexemExecution(argument, lexem);

            return null;
        }
    }
}
