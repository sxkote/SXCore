using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SXCore.Lexems
{
    public interface IEnvironment
    {
        ICollection<SXLexemVariable> Variables { get; }

        SXLexemVariable Get(string name);
        SXLexemVariable Set(string name, SXLexemValue value);

        SXLexemVariable Calculate(SXLexemFunction function);
    }

    public class SXEnvironment : IEnvironment
    {
        public delegate SXLexemVariable OnFunctionExecuting(SXLexemFunction function);

        #region Variables
        protected List<SXLexemVariable> _variables = new List<SXLexemVariable>();
        protected OnFunctionExecuting onFunctionExecuting = null;
        #endregion

        #region Properties
        public ICollection<SXLexemVariable> Variables
        { get { return _variables; } }

        public OnFunctionExecuting FunctionExecuting
        {
            get { return onFunctionExecuting; }
            set { onFunctionExecuting = value; }
        }

        public SXLexemVariable this[string name]
        {
            get
            {
                return this.Get(name);
            }
            set
            {
                if (value == null || String.IsNullOrEmpty(name))
                    return;

                var variable = this.Variables.FirstOrDefault(v => v.Name == name);

                if (variable == null)
                    this.Variables.Add(value);
                else
                    variable.Value = value.Value;
            }
        }
        #endregion

        #region Constructors
        public SXEnvironment() { }

        public SXEnvironment(IEnumerable<SXLexemVariable> variables)
        { _variables = variables.ToList(); }
        #endregion

        #region Functions
        public SXLexemVariable Get(string name)
        {
            return this.Variables.FirstOrDefault(v => v.Name == name);
        }

        public SXLexemVariable Set(string name, SXLexemValue value)
        {
            if (value == null || String.IsNullOrEmpty(name))
                return null;

            var variable = this.Get(name);
            if (variable == null)
            {
                variable = new SXLexemVariable(name);
                this.Variables.Add(variable);
            }

            variable.Value = value;

            return variable;
        }

        public SXLexemVariable Add(string name, SXLexemValue value)
        {
            return this.Set(name, value);
        }
        #endregion

        #region Calculations
        public virtual SXLexemVariable Calculate(SXLexemFunction function)
        {
            if (this.FunctionExecuting != null)
                return this.FunctionExecuting(function);

            return null;
        }
        #endregion
    }
}
